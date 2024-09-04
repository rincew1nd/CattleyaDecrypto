using CattleyaDecrypto.Server.Architecture;
using CattleyaDecrypto.Server.Models.Enums;
using CattleyaDecrypto.Server.Models.Models;
using CattleyaDecrypto.Server.Models.ViewModels;
using CattleyaDecrypto.Server.Services.Interfaces;
using CattleyaDecrypto.Server.Services.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace CattleyaDecrypto.Server.Services;

/// <summary>
/// Service to process events for Decrypto match.
/// </summary>
public class DecryptoMatchService : IDecryptoMatchService
{
    private readonly IWordPuzzleService _wordPuzzleService;
    private readonly IMemoryCache _memoryCache;
    private readonly IHubContext<DecryptoMessageHub> _decryptoMessageHub;

    public DecryptoMatchService(
        IWordPuzzleService wordPuzzleService,
        IMemoryCache memoryCache,
        IHubContext<DecryptoMessageHub> decryptoMessageHub)
    {
        _wordPuzzleService = wordPuzzleService;
        _memoryCache = memoryCache;
        _decryptoMessageHub = decryptoMessageHub;
    }
    
    /// <summary>
    /// Create new match.
    /// </summary>
    public DecryptoMatch CreateMatch()
    {
        var match = DecryptoMatch.Default;

        foreach (var team in match.Teams.Values)
        {
            var words = _wordPuzzleService.PuzzleWords(4);
            foreach (var wordWithIndex in words.Select((val, index) => (val, index)))
            {
                team.Words.Add(wordWithIndex.index, wordWithIndex.val);
                team.Clues.Add(wordWithIndex.index, []);
            }
        }

        _memoryCache.Set(match.Id, match, new MemoryCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
        });

        return match;
    }
    
    /// <summary>
    /// Get full state of a match.
    ///
    /// TODO Think about moving deep copy of an object to frontend.
    /// </summary>
    public DecryptoMatch GetMatch(Guid matchId, Guid userId)
    {
        var match = GetMatchFromCache(matchId);

        // If match is over, do not deep copy it.
        if (match.State == DecryptMatchState.Finished)
        {
            return match;
        }
        
        // Deep copy a match so it could be changed.
        var matchCopy = GetMatchFromCache(matchId).CloneJson();
        var team = matchCopy!.Teams.Where(t => t.Value.Players.ContainsKey(userId)).Select(t => t.Key).FirstOrDefault();

        // Clear words of opposite team, so the opponent can't see them.
        var oppositeTeam = OppositeTeam(team);
        foreach (var (key, value) in matchCopy.Teams)
        {
            if (oppositeTeam == null || oppositeTeam == key)
            {
                value.Words.Clear();
            }
        }

        return matchCopy;
    }
    
    /// <summary>
    /// Join a match.
    /// </summary>
    public async Task<bool> JoinTeamAsync(Guid matchId, TeamEnum team, Guid userId, string userName)
    {
        var match = GetMatchFromCache(matchId);
        if (match.Teams.SelectMany(t => t.Value.Players.Keys).Contains(userId))
        {
            return false;
        }
        match.Teams[team].Players.Add(userId, userName);

        await UpdateMatchState(match, DecryptMatchState.WaitingForPlayers);
        
        return true;
    }

    /// <summary>
    /// Give clues.
    /// </summary>
    public async Task<bool> GiveCluesAsync(GiveCluesVm model, Guid userId)
    {
        var match = GetMatchFromCache(model.MatchId);

        if (match.State != DecryptMatchState.GiveClues)
        {
            throw new ApplicationException($"Wrong state for 'Give clues' action - {match.State}");
        }

        AssertPlayerInTeam(match, model.Team, userId);

        var result = match.TemporaryClues.TryAdd(
            model.Team,
            new CluesToSolve
            {
                Order = model.Order,
                Clues = model.Clues
            });

        await UpdateMatchState(match, DecryptMatchState.GiveClues);
        
        return result;
    }

    /// <summary>
    /// Solve clues.
    /// </summary>
    public async Task SolveCluesASync(SolveOrInterceptCluesVm model, Guid userId)
    {
        var match = GetMatchFromCache(model.MatchId);

        if (match.State != DecryptMatchState.SolveClues)
        {
            throw new ApplicationException($"Wrong state for 'SolveClues' action - {match.State}");
        }
        
        AssertPlayerInTeam(match, model.Team, userId);
        
        if (!match.TemporaryClues.TryGetValue(model.Team, out var cluesToSolve))
        {
            throw new ApplicationException("Could not solve unriddle clues");
        }

        match.TemporaryClues[model.Team].IsSolved = true;
        if (cluesToSolve.Order != model.Order)
        {
            match.Teams[model.Team].MiscommunicationCount++;
        }
        
        await UpdateMatchState(match, DecryptMatchState.SolveClues);
    }

    /// <summary>
    /// Intercept clues.
    /// </summary>
    public async Task InterceptASync(SolveOrInterceptCluesVm model, Guid userId)
    {
        var enemyTeam = OppositeTeam(model.Team);
        if (!enemyTeam.HasValue)
        {
            throw new ApplicationException("Player is not in a team");
        }
        
        var match = GetMatchFromCache(model.MatchId);

        if (match.State != DecryptMatchState.Intercept)
        {
            throw new ApplicationException($"Wrong state for 'Intercept' action - {match.State}");
        }
        
        AssertPlayerInTeam(match, model.Team, userId);
        
        if (!match.TemporaryClues.TryGetValue(enemyTeam.Value, out var cluesToSolve))
        {
            throw new ApplicationException("Could not solve unriddle clues");
        }

        match.TemporaryClues[enemyTeam.Value].IsIntercepted = true;
        if (cluesToSolve.Order == model.Order)
        {
            match.Teams[model.Team].InterceptionCount++;
        }
        
        await UpdateMatchState(match, DecryptMatchState.SolveClues);
    }

    public async Task SendMatchUpdate(Guid matchId)
    {
        var match = GetMatchFromCache(matchId);
        await _decryptoMessageHub.Clients.Group(match.Id.ToString()).SendAsync("StateChanged", match);
    }

    /// <summary>
    /// Get full state of a match.
    /// </summary>
    private DecryptoMatch GetMatchFromCache(Guid matchId)
    {
        if (_memoryCache.TryGetValue<DecryptoMatch>(matchId, out var match))
        {
            return match!;
        }
        throw new ApplicationException("Match not found");
    }

    /// <summary>
    /// Assert that the player is in the right team.
    /// </summary>
    private void AssertPlayerInTeam(DecryptoMatch match, TeamEnum team, Guid userId)
    {
        if (!match.Teams[team].Players.ContainsKey(userId))
        {
            throw new ApplicationException("Player is in opposite team.");
        }
    }

    /// <summary>
    /// Update a state of a match. Notify players about the change.
    /// </summary>
    private async Task<bool> UpdateMatchState(DecryptoMatch match, DecryptMatchState currentState)
    {
        bool stateChanged = false;
        
        switch (currentState)
        {
            case DecryptMatchState.WaitingForPlayers:
            {
                if (match.Teams.All(t => t.Value.Players.Any()))
                {
                    match.State = DecryptMatchState.GiveClues;
                    stateChanged = true;
                }
                break;
            }
            case DecryptMatchState.GiveClues:
            {
                if (match.TemporaryClues.Count == 2)
                {
                    match.State = DecryptMatchState.SolveClues;
                    stateChanged = true;
                }
                break;
            }
            case DecryptMatchState.SolveClues:
            {
                if (match.TemporaryClues.All(x => x.Value.IsSolved))
                {
                    if (match.Round == 1)
                    {
                        match.State = DecryptMatchState.GiveClues;
                        match.Round++;
                    }
                    match.State = DecryptMatchState.Intercept;
                    stateChanged = true;
                }
                break;
            }
            case DecryptMatchState.Intercept:
            {
                if (match.TemporaryClues.All(x => x.Value.IsIntercepted))
                {
                    TeamEnum? wonTeam = null;
                    if (match.Teams[TeamEnum.Blue].MiscommunicationCount == 2 ||
                        match.Teams[TeamEnum.Red].InterceptionCount == 2)
                    {
                        wonTeam = TeamEnum.Red;
                    }
                    if (match.Teams[TeamEnum.Red].MiscommunicationCount == 2 ||
                        match.Teams[TeamEnum.Blue].InterceptionCount == 2)
                    {
                        wonTeam = TeamEnum.Blue;
                    }

                    match.WonTeam = wonTeam;
                    match.State = wonTeam.HasValue ? DecryptMatchState.Finished : DecryptMatchState.GiveClues;

                    foreach (var temporaryClue in match.TemporaryClues)
                    {
                        foreach (var clues in temporaryClue.Value.Clues)
                        {
                            match.Teams[temporaryClue.Key].Clues[clues.Key].Add(clues.Value);
                        }
                    }
                    match.TemporaryClues.Clear();
                    
                    stateChanged = true;
                }
                break;
            }
        }

        if (stateChanged)
        {
            await _decryptoMessageHub.Clients.Group(match.Id.ToString()).SendAsync("StateChanged", match);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Get opposite teams. If 'Unknown', both are opposite.
    /// </summary>
    private TeamEnum? OppositeTeam(TeamEnum team)
    {
        return team switch
        {
            TeamEnum.Blue => TeamEnum.Red,
            TeamEnum.Red => TeamEnum.Blue,
            _ => null
        };
    }
}