using System.ComponentModel.DataAnnotations.Schema;
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
    public TeamEnum? WonTeam { get; set; }

    /// <summary>
    /// Bit representation of all available variants of word order.
    /// </summary>
    public int AvailableWordOrders = 16777215;
    
    /// <summary>
    /// Team information.
    /// </summary>
    public Dictionary<TeamEnum, TeamInfoModel> Teams { get; set; }

    /// <summary>
    /// Clues to solve.
    /// </summary>
    public Dictionary<TeamEnum, CluesToSolve> TemporaryClues { get; set; } = new();

    /// <summary>
    /// Get default decrypto match.
    /// </summary>
    [NotMapped]
    public static DecryptoMatch Default =>
        new()
        {
            Id = Guid.NewGuid(),
            Round = 1,
            State = DecryptMatchState.WaitingForPlayers,
            Teams = new Dictionary<TeamEnum, TeamInfoModel>
            {
                {
                    TeamEnum.Blue,
                    new TeamInfoModel()
                },
                {
                    TeamEnum.Red,
                    new TeamInfoModel()
                }
            }
        };
}