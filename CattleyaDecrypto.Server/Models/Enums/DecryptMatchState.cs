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
    /// Waiting for clues to be submitted.
    /// </summary>
    GiveClues = 1,
    
    /// <summary>
    /// Team is solving the clues.
    /// </summary>
    SolveClues = 2,
    
    /// <summary>
    /// Intercept enemy team.
    /// </summary>
    Intercept = 3,
    
    /// <summary>
    /// Match is finished.
    /// </summary>
    Finished = 4
}