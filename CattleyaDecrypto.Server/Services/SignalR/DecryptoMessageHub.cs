using Microsoft.AspNetCore.SignalR;

namespace CattleyaDecrypto.Server.Services.SignalR;

/// <summary>
/// SignalR message hub for Decrypto notifications.
/// </summary>
public class DecryptoMessageHub : Hub
{
    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"{Context.ConnectionId} connected");
        return base.OnConnectedAsync();
    }
    
    /// <summary>
    /// Subscribe to updates of a match.
    /// </summary>
    public Task JoinMatch(string matchId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, matchId);
    }
    
    /// <summary>
    /// Unsubscribe from updates of a match.
    /// </summary>
    public Task LeaveMatch(string matchId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, matchId);
    }
}