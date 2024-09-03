using Microsoft.AspNetCore.SignalR;

namespace CattleyaDecrypto.Server.Services.SignalR;

/// <summary>
/// Message hub for SignalR.
/// </summary>
public class MessageHub : Hub
{
    /// <summary>
    /// Send test message to clients.
    /// </summary>
    public async Task SendTestMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage");
    }
}