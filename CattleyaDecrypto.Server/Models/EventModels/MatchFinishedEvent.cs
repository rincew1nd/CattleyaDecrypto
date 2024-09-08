using CattleyaDecrypto.Server.Models.Enums;

namespace CattleyaDecrypto.Server.Models.EventModels;

public class MatchFinishedEvent
{
    /// <summary>
    /// Won team, possible Unknown.
    /// </summary>
    public DecryptoTeamEnum Team { get; set; }
    
    /// <summary>
    /// Opponent words.
    /// </summary>
    public string[] Words { get; set; }
}