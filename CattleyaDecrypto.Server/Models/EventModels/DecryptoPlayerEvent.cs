using CattleyaDecrypto.Server.Models.Enums;

namespace CattleyaDecrypto.Server.Models.EventModels;

/// <summary>
/// Player joined event.
/// </summary>
public class DecryptoPlayerEvent
{
    /// <summary>
    /// Player Id.
    /// </summary>
    public Guid PlayerId { get; set; }
    
    /// <summary>
    /// Player Name.
    /// </summary>
    public string PlayerName { get; set; }
    
    /// <summary>
    /// Player Team.
    /// </summary>
    public TeamEnum Team { get; set; }
}