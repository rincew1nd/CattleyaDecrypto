using CattleyaDecrypto.Server.Models.Enums;

namespace CattleyaDecrypto.Server.Models.EventModels;

public class PrepareClueEvent
{
    /// <summary>
    /// Team
    /// </summary>
    public DecryptoTeamEnum Team { get; set; }
    
    /// <summary>
    /// Clues.
    /// </summary>
    public string[] Clues { get; set; }
}