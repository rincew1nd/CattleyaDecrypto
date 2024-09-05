namespace CattleyaDecrypto.Server.Models.Models;

/// <summary>
/// Clues to solve.
/// </summary>
public class CluesToSolve
{
    /// <summary>
    /// Correct order of clues.
    /// </summary>
    public int[] Order { get; set; }
    
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