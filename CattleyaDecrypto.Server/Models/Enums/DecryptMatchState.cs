namespace CattleyaDecrypto.Server.Models.Enums;

/// <summary>
/// Possible states of Decrypto match.
/// </summary>
public enum DecryptMatchState : short
{
    /// <summary>
    /// Match created, waiting for players to start.
    /// </summary>
    WaitingForPlayers = 0,
    
    /// <summary>
    /// Waiting for players to decide who will give clues and send them.
    /// </summary>
    GiveClues = 1,
    
    /// <summary>
    /// Team is solving the clues.
    /// </summary>
    SolveClues = 2,
    
    /// <summary>
    /// Intercept opponent team.
    /// </summary>
    Intercept = 3,
    
    /// <summary>
    /// Match is finished.
    /// </summary>
    Finished = 4
}