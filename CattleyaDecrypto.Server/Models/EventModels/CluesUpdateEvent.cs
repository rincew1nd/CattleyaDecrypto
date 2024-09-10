using CattleyaDecrypto.Server.Models.Enums;

namespace CattleyaDecrypto.Server.Models.EventModels;

public class CluesUpdateEvent
{
    public DecryptMatchState MatchState { get; set; }
    public DecryptoTeamEnum Team { get; set; }
    public int Miscommunications { get; set; }
    public int Interceptions { get; set; }
}