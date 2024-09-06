namespace CattleyaDecrypto.Server.Models.EventModels;

/// <summary>
/// Player joined event.
/// </summary>
public class DecryptoAssignRiddlerEvent : DecryptoPlayerEvent
{
    /// <summary>
    /// Order of words.
    /// </summary>
    public int[] Order { get; set; }
}