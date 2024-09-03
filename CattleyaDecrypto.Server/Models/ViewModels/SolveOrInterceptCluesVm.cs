using CattleyaDecrypto.Server.Models.Enums;

namespace CattleyaDecrypto.Server.Models.ViewModels;

/// <summary>
/// ViewModel for solving clues.
/// </summary>
public class SolveOrInterceptCluesVm
{
    /// <summary>
    /// Match ID.
    /// </summary>
    public Guid MatchId { get; set; }
    
    /// <summary>
    /// Team.
    /// </summary>
    public TeamEnum Team { get; set; }
    
    /// <summary>
    /// Given order of clues.
    /// </summary>
    public int[] Order { get; set; }
}