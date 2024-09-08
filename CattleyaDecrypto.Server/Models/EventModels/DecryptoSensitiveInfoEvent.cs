namespace CattleyaDecrypto.Server.Models.EventModels;

public class DecryptoSensitiveInfoEvent
{
    public string[] Words { get; set; }
    public int[] RoundWordOrder { get; set; }
}