namespace CattleyaDecrypto.Server.Models.Models;

/// <summary>
/// Team information.
/// </summary>
public class TeamInfoModel
{
    /// <summary>
    /// Count of miscommunications.
    /// </summary>
    public int MiscommunicationCount { get; set; }

    /// <summary>
    /// Count of interceptions.
    /// </summary>
    public int InterceptionCount { get; set; }
    
    /// <summary>
    /// Dictionary for words.
    /// </summary>
    public Dictionary<int, string> Words { get; set; } = new();
    
    /// <summary>
    /// Dictionary for clues.
    /// </summary>
    public Dictionary<int, List<string>> Clues { get; set; } = new();
    
    /// <summary>
    /// Player list.
    /// </summary>
    public Dictionary<Guid, string> Players { get; set; } = new();
}