using CattleyaDecrypto.Server.Features.Decrypto.ChangePlayerName;
using CattleyaDecrypto.Server.Features.Decrypto.PlayerJoinedMatch;
using CattleyaDecrypto.Server.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CattleyaDecrypto.Server.Services.SignalR;

/// <summary>
/// SignalR message hub for Decrypto notifications.
/// </summary>
public class DecryptoMessageHub : Hub
{
    private readonly IMediator _mediator;

    public DecryptoMessageHub(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Subscribe to updates of a match.
    /// </summary>
    public async Task JoinMatch(Guid matchId)
    {
        await _mediator.Send(new PlayerJoinedMatchCommand(matchId));
        await Groups.AddToGroupAsync(Context.ConnectionId, matchId.ToString());
    }
    
    /// <summary>
    /// Unsubscribe from updates of a match.
    /// </summary>
    public Task LeaveMatch(Guid matchId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, matchId.ToString());
    }

    /// <summary>
    /// Announce player name change.
    /// </summary>
    /// <param name="matchId">Match ID</param>
    public async Task NameChanged(Guid matchId)
    {
        await _mediator.Send(new ChangePlayerNameCommand(matchId));
    }
}