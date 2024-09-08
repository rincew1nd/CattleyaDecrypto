using CattleyaDecrypto.Server.Models.Enums;

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
    public Dictionary<int, CreateNewMatchTeamResponse> Teams { get; set; }
}

public class CreateNewMatchTeamResponse
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
