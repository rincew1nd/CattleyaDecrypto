namespace CattleyaDecrypto.Server.Models.Enums;

/// <summary>
/// Possible states of Decrypto match.
/// </summary>
public enum DecryptMatchState : short
{
    /// <summary>
    /// Match created, waiting for players to start.
    /// </summary>
    WaitingForPlayers,
    
    /// <summary>
    /// Waiting for clues to be submitted.
    /// </summary>
    GiveClues,
    
    /// <summary>
    /// Team is solving the clues.
    /// </summary>
    SolveClues,
    
    /// <summary>
    /// Intercept enemy team.
    /// </summary>
    Intercept,
    
    /// <summary>
    /// Match is finished.
    /// </summary>
    Finished
}