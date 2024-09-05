using CattleyaDecrypto.Server.Models.Enums;

namespace CattleyaDecrypto.Server.Models.ViewModels;

/// <summary>
/// ViewModel for giving clues.
/// </summary>
public class GiveCluesVm
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
    /// Correct order of clues.
    /// </summary>
    public int[] Order { get; set; }
    
    /// <summary>
    /// List of clues.
    /// </summary>
    public string[] Clues { get; set; }
}