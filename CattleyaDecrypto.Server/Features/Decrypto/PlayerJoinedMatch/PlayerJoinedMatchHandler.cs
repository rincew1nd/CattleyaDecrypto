using CattleyaDecrypto.Server.Services.Interfaces;
using CattleyaDecrypto.Server.Services.SignalR;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CattleyaDecrypto.Server.Features.Decrypto.PlayerJoinedMatch;

public class PlayerJoinedMatchHandler : DecryptoBaseHandler, IRequestHandler<PlayerJoinedMatchCommand>
{
    private readonly IUserContextService _userContextService;

    public PlayerJoinedMatchHandler(
        IUserContextService userContextService,
        ICacheService cacheService,
        IHubContext<DecryptoMessageHub> decryptoMessageHub)
        : base(cacheService, decryptoMessageHub)
    {
        _userContextService = userContextService;
    }

    public async Task Handle(PlayerJoinedMatchCommand request, CancellationToken cancellationToken)
    {
        var match = await GetMatchAsync(request.MatchId);

        var team = match.Teams.FirstOrDefault(t => t.Value.Players.ContainsKey(_userContextService.GetId()));

        if (team.Key != default)
        {
            await _decryptoMessageHub.Clients
                .User(_userContextService.GetId().ToString())
                .SendAsync(
                    "SetTeamWords",
                    match.Teams[team.Key].Words,
                    cancellationToken);
        }
    }
}