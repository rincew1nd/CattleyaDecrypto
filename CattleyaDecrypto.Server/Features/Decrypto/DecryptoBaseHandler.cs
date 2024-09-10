using System.Text.RegularExpressions;
using CattleyaDecrypto.Server.Models.Enums;
using CattleyaDecrypto.Server.Models.EventModels;
using CattleyaDecrypto.Server.Models.Models;
using CattleyaDecrypto.Server.Services.Interfaces;
using CattleyaDecrypto.Server.Services.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace CattleyaDecrypto.Server.Features.Decrypto;

public abstract class DecryptoBaseHandler
{
    private readonly ICacheService _cacheService;
    protected readonly IHubContext<DecryptoMessageHub> _decryptoMessageHub;

    protected DecryptoBaseHandler(ICacheService cacheService, IHubContext<DecryptoMessageHub> decryptoMessageHub)
    {
        _cacheService = cacheService;
        _decryptoMessageHub = decryptoMessageHub;
    }
    
    /// <summary>
    /// Get a match from cache.
    /// </summary>
    protected async Task<DecryptoMatch> GetMatchAsync(Guid matchId)
    {
        var match = await _cacheService.GetRecordAsync<DecryptoMatch>(_cacheService.GetKey<Match>(matchId));
        if (match != default)
        {
            return match!;
        }
        throw new ApplicationException("Match not found");
    }
    
    /// <summary>
    /// Execute match update with record lock (non-generic).
    /// </summary>
    protected async Task MatchUpdate(Guid matchId, Func<DecryptoMatch, Task> action)
    {
        await using var redLock = await _cacheService.LockRecord(_cacheService.GetKey<Match>(matchId));
        
        var match = await GetMatchAsync(matchId);

        await action(match);
        
        await _cacheService.SetRecordAsync(_cacheService.GetKey<Match>(matchId), match);
    }
    
    /// <summary>
    /// Assert that the player is in the right team.
    /// </summary>
    protected void AssertPlayerInTeam(DecryptoMatch match, DecryptoTeamEnum decryptoTeam, Guid userId)
    {
        if (!match.Teams[decryptoTeam].Players.ContainsKey(userId))
        {
            throw new ApplicationException("Player is in opposite team.");
        }
    }
    
    /// <summary>
    /// Assert that the player is in the right team.
    /// </summary>
    protected DecryptoTeamEnum GetPlayerTeam(DecryptoMatch match, Guid userId)
    {
        return match.Teams
            .Where(t => t.Value.Players.ContainsKey(userId))
            .Select(t => t.Key)
            .FirstOrDefault();
    }

    /// <summary>
    /// Update a state of a match. Notify players about the change.
    /// </summary>
    protected async Task<bool> UpdateMatchState(
        DecryptoMatch match,
        DecryptMatchState currentState,
        CancellationToken cancellationToken)
    {
        bool stateChanged = false;
        
        switch (currentState)
        {
            case DecryptMatchState.WaitingForPlayers:
            {
                if (match.Teams.All(t => t.Value.Players.Any()) && match.State == DecryptMatchState.WaitingForPlayers)
                {
                    match.State = DecryptMatchState.GiveClues;
                    stateChanged = true;
                }
                break;
            }
            case DecryptMatchState.GiveClues:
            {
                if (match.RoundClues.Keys.Count == 2
                    && match.RoundClues.All(tc => tc.Value.Clues?.Any() ?? false))
                {
                    match.State = DecryptMatchState.SolveClues;
                    stateChanged = true;
                }
                break;
            }
            case DecryptMatchState.SolveClues:
            {
                if (match.RoundClues.All(x => x.Value.IsSolved))
                {
                    foreach (var temporaryClue in match.RoundClues)
                    {
                        for (var i = 0; i < temporaryClue.Value.Clues.Length; i++)
                        {
                            if (!match.Teams[temporaryClue.Key].Clues.ContainsKey(temporaryClue.Value.Order[i]))
                            {
                                match.Teams[temporaryClue.Key].Clues[temporaryClue.Value.Order[i]] = new();
                            }
                            match
                                .Teams[temporaryClue.Key]
                                .Clues[temporaryClue.Value.Order[i]]
                                .Add(temporaryClue.Value.Clues[i]);
                        }
                    }
                    
                    if (match.Round == 1)
                    {
                        match.State = DecryptMatchState.GiveClues;
                        match.Round++;
                    }
                    else
                    {
                        match.State = DecryptMatchState.Intercept;
                        match.RoundClues.Clear();
                    }
                    
                    stateChanged = true;
                }
                break;
            }
            case DecryptMatchState.Intercept:
            {
                if (match.RoundClues.All(x => x.Value.IsIntercepted))
                {
                    var matchWon = false;
                    
                    if (match.Teams[DecryptoTeamEnum.Blue].MiscommunicationCount == 2 ||
                        match.Teams[DecryptoTeamEnum.Red].InterceptionCount == 2)
                    {
                        match.WonTeam = DecryptoTeamEnum.Red;
                        matchWon = true;
                    }
                    if (match.Teams[DecryptoTeamEnum.Red].MiscommunicationCount == 2 ||
                        match.Teams[DecryptoTeamEnum.Blue].InterceptionCount == 2)
                    {
                        match.WonTeam = match.WonTeam == DecryptoTeamEnum.Red
                            ? DecryptoTeamEnum.Unknown
                            : DecryptoTeamEnum.Blue;
                        matchWon = true;
                    }

                    match.State = DecryptMatchState.GiveClues;
                    if (matchWon || match.Round == 10)
                    {
                        match.State = DecryptMatchState.Finished;
                        await _decryptoMessageHub.Clients
                            .Group(match.Id.ToString())
                            .SendAsync("MatchFinished", match.WonTeam, cancellationToken);
                    }

                    match.RoundClues.Clear();
                    
                    stateChanged = true;
                }
                break;
            }
        }

        if (stateChanged)
        {
            await _decryptoMessageHub.Clients.Group(match.Id.ToString()).SendAsync("StateChanged", match.State);
        }

        return stateChanged;
    }

    /// <summary>
    /// Get opposite teams. If 'Unknown', both are opposite.
    /// </summary>
    protected DecryptoTeamEnum OppositeTeam(DecryptoTeamEnum decryptoTeam)
    {
        return decryptoTeam switch
        {
            DecryptoTeamEnum.Blue => DecryptoTeamEnum.Red,
            DecryptoTeamEnum.Red => DecryptoTeamEnum.Blue,
            _ => DecryptoTeamEnum.Unknown
        };
    }

    /// <summary>
    /// Send sensitive info to clients.
    /// </summary>
    protected async Task SendSensitiveInfoAsync(
        Guid userId, DecryptoMatch match, DecryptoTeamEnum team, CancellationToken cancellationToken)
    {
        await _decryptoMessageHub.Clients
            .User(userId.ToString())
            .SendAsync(
                "SensitiveInfo",
                new DecryptoSensitiveInfoEvent()
                {
                    Words = match.Teams[team].Words,
                    RoundWordOrder = match.RoundClues.ContainsKey(team) &&
                                     match.RoundClues[team].RiddlerId == userId
                        ? match.RoundClues[team].Order
                        : []
                },
                cancellationToken);
    }

    protected async Task ScoreAndStateUpdateEvent(
        Guid matchId,
        DecryptMatchState state,
        DecryptoTeamEnum team,
        int miscommunicationCount,
        int interceptionCount,
        CancellationToken cancellationToken)
    {
        await _decryptoMessageHub.Clients
            .Group(matchId.ToString())
            .SendAsync(
                "ScoreAndStateUpdate",
                new CluesUpdateEvent
                {
                    MatchState = state,
                    Team = team,
                    Miscommunications = miscommunicationCount,
                    Interceptions = interceptionCount
                },
                cancellationToken);
    }
    
    protected async Task CluesUpdateEvent(
        DecryptoMatch match,
        CancellationToken cancellationToken)
    {
        await _decryptoMessageHub.Clients
            .Group(match.Id.ToString())
            .SendAsync(
                "CluesUpdate",
                match.Teams.ToDictionary(kv => kv.Key, kv => kv.Value.Clues),
                cancellationToken);
    }
}