using CattleyaDecrypto.Server.Models.Enums;

namespace CattleyaDecrypto.Server.Models.Models;

/// <summary>
/// Match info.
/// </summary>
public class DecryptoMatch
{
    /// <summary>
    /// ID.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Round number.
    /// </summary>
    public int Round { get; set; }
    
    /// <summary>
    /// Match state.
    /// </summary>
    public DecryptMatchState State { get; set; }

    /// <summary>
    /// Team that won the match.
    /// </summary>
    public DecryptoTeamEnum WonTeam { get; set; } = DecryptoTeamEnum.Unknown;

    /// <summary>
    /// Bit representation of all available variants of word order.
    /// </summary>
    public int AvailableWordOrders = 16777215;

    /// <summary>
    /// Team information.
    /// </summary>
    public Dictionary<DecryptoTeamEnum, TeamInfoModel> Teams { get; set; } = new();

    /// <summary>
    /// Clues to solve.
    /// </summary>
    public Dictionary<DecryptoTeamEnum, CluesToSolve> RoundClues { get; set; } = new();
}