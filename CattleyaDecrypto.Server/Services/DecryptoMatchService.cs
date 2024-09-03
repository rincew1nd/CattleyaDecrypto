using CattleyaDecrypto.Server.Models.Enums;
using CattleyaDecrypto.Server.Models.Models;
using CattleyaDecrypto.Server.Models.ViewModels;
using CattleyaDecrypto.Server.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace CattleyaDecrypto.Server.Services;

/// <summary>
/// Service to process events for Decrypto match.
/// </summary>
public class DecryptoMatchService : IDecryptoMatchService
{
    private readonly IWordPuzzleService _wordPuzzleService;
    private readonly IMemoryCache _memoryCache;

    public DecryptoMatchService(IWordPuzzleService wordPuzzleService, IMemoryCache memoryCache)
    {
        _wordPuzzleService = wordPuzzleService;
        _memoryCache = memoryCache;
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
    /// </summary>
    public DecryptoMatch GetMatch(Guid matchId)
    {
        if (_memoryCache.TryGetValue(matchId, out var match))
        {
            return (match as DecryptoMatch)!;
        }
        throw new ApplicationException("Match not found");
    }
    
    /// <summary>
    /// Join a match.
    /// </summary>
    public bool JoinTeam(Guid matchId, TeamEnum team, Guid userId)
    {
        //TODO Notify about player connection.
        
        var match = GetMatch(matchId);
        if (match.Teams.SelectMany(t => t.Value.Players).Contains(userId))
        {
            return false;
        }
        match.Teams[team].Players.Add(userId);
        return true;
    }

    /// <summary>
    /// Give clues.
    /// </summary>
    public bool GiveClues(GiveCluesVm model)
    {
        var match = GetMatch(model.MatchId);

        if (match.State != DecryptMatchState.GiveClues)
        {
            throw new ApplicationException($"Wrong state for 'Give clues' action - {match.State}");
        }

        var result = match.TemporaryClues.TryAdd(
            model.Team,
            new CluesToSolve()
            {
                Order = model.Order,
                Clues = model.Clues
            });

        UpdateMatchState(match, DecryptMatchState.GiveClues);
        return result;
    }

    /// <summary>
    /// Solve clues.
    /// </summary>
    public void SolveClues(SolveOrInterceptCluesVm model)
    {
        var match = GetMatch(model.MatchId);

        if (match.State != DecryptMatchState.SolveClues)
        {
            throw new ApplicationException($"Wrong state for 'SolveClues' action - {match.State}");
        }
        
        if (!match.TemporaryClues.TryGetValue(model.Team, out var cluesToSolve))
        {
            throw new ApplicationException("Could not solve unriddle clues");
        }

        match.TemporaryClues[model.Team].IsSolved = true;
        if (cluesToSolve.Order != model.Order)
        {
            match.Teams[model.Team].MiscommunicationCount++;
        }
        
        UpdateMatchState(match, DecryptMatchState.SolveClues);
    }

    /// <summary>
    /// Intercept clues.
    /// </summary>
    public void Intercept(SolveOrInterceptCluesVm model)
    {
        var enemyTeam = model.Team == TeamEnum.Blue ? TeamEnum.Red : TeamEnum.Blue;
        
        var match = GetMatch(model.MatchId);

        if (match.State != DecryptMatchState.Intercept)
        {
            throw new ApplicationException($"Wrong state for 'Intercept' action - {match.State}");
        }
        
        if (!match.TemporaryClues.TryGetValue(enemyTeam, out var cluesToSolve))
        {
            throw new ApplicationException("Could not solve unriddle clues");
        }

        match.TemporaryClues[enemyTeam].IsIntercepted = true;
        if (cluesToSolve.Order == model.Order)
        {
            match.Teams[model.Team].InterceptionCount++;
        }
        
        UpdateMatchState(match, DecryptMatchState.SolveClues);
    }

    private bool UpdateMatchState(DecryptoMatch match, DecryptMatchState currentState)
    {
        bool stateChanged = false;
        
        switch (currentState)
        {
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
            //TODO Notify about match state change.
            return true;
        }

        return false;
    }
}