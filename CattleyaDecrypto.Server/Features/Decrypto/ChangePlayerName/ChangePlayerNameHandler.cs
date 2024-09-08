using CattleyaDecrypto.Server.Models.EventModels;
using CattleyaDecrypto.Server.Services.Interfaces;
using CattleyaDecrypto.Server.Services.SignalR;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CattleyaDecrypto.Server.Features.Decrypto.ChangePlayerName;

public class ChangePlayerNameHandler : DecryptoBaseHandler, IRequestHandler<ChangePlayerNameCommand>
{
    private readonly IUserContextService _userContextService;

    public ChangePlayerNameHandler(
        IUserContextService userContextService,
        ICacheService cacheService,
        IHubContext<DecryptoMessageHub> decryptoMessageHub)
        : base(cacheService, decryptoMessageHub)
    {
        _userContextService = userContextService;
    }

    public async Task Handle(ChangePlayerNameCommand request, CancellationToken cancellationToken)
    {
        await MatchUpdate(request.MatchId, async match =>
        {
            var userInfo = await _userContextService.GetUserInfo();

            var team = match.Teams.FirstOrDefault(t => t.Value.Players.ContainsKey(userInfo.Id));
            team.Value.Players[userInfo.Id] = userInfo.Name;

            await _decryptoMessageHub.Clients
                .Group(request.MatchId.ToString())
                .SendAsync(
                    "NameChanged",
                    new DecryptoPlayerEvent
                    {
                        PlayerId = userInfo.Id,
                        PlayerName = userInfo.Name,
                        Team = team.Key
                    },
                    cancellationToken);
        });
    }
}