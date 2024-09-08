using CattleyaDecrypto.Server.Models.Enums;
using CattleyaDecrypto.Server.Models.Models;

namespace CattleyaDecrypto.Server.Features.Decrypto;

public class DecryptoMatchResponse
{
    /// <summary>
    /// ID.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Round.
    /// </summary>
    public int Round { get; set; }
    
    /// <summary>
    /// State.
    /// </summary>
    public DecryptMatchState State { get; set; }

    /// <summary>
    /// Teams.
    /// </summary>
    public Dictionary<int, DecryptoMatchTeamResponse> Teams { get; set; } = new();

    /// <summary>
    /// Clues for a round.
    /// </summary>
    public Dictionary<int, DecryptoMatchCluesResponse> RoundClues { get; set; } = new();

    /// <summary>
    /// .ctor
    /// </summary>
    public DecryptoMatchResponse(DecryptoMatch match)
    {
        Id = match.Id;
        Round = match.Round;
        State = match.State;
        
        foreach (var (key, value) in match.Teams)
        {
            Teams.Add((int)key, new DecryptoMatchTeamResponse()
            {
                InterceptionCount = value.InterceptionCount,
                MiscommunicationCount = value.MiscommunicationCount,
                Players = value.Players
            });
        }
        
        foreach (var (key, value) in match.RoundClues)
        {
            RoundClues.Add((int)key, new DecryptoMatchCluesResponse
            {
                RiddlerId = value.RiddlerId,
                IsIntercepted = value.IsIntercepted,
                IsSolved = value.IsSolved,
                Clues = value.Clues
            });
        }
    }
}

public class DecryptoMatchTeamResponse
{
    /// <summary>
    /// Interception count.
    /// </summary>
    public int InterceptionCount { get; set; }
    
    /// <summary>
    /// Miscommunication count.
    /// </summary>
    public int MiscommunicationCount { get; set; }
    
    /// <summary>
    /// Players.
    /// </summary>
    public Dictionary<Guid, string> Players { get; set; }
}

public class DecryptoMatchCluesResponse
{
    /// <summary>
    /// Clues.
    /// </summary>
    public string[] Clues { get; set; }
    
    /// <summary>
    /// Player ID who riddled clues.
    /// </summary>
    public Guid RiddlerId { get; set; }

    /// <summary>
    /// Was clues been solved.
    /// </summary>
    public bool IsSolved { get; set; }
    
    /// <summary>
    /// Was clues been intercepted.
    /// </summary>
    public bool IsIntercepted { get; set; }
}