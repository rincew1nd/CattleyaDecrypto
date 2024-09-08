using CattleyaDecrypto.Server.Models.Enums;
using CattleyaDecrypto.Server.Models.EventModels;
using CattleyaDecrypto.Server.Services.Interfaces;
using CattleyaDecrypto.Server.Services.SignalR;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CattleyaDecrypto.Server.Features.Decrypto.JoinTeam;

public class JoinTeamHandler : DecryptoBaseHandler, IRequestHandler<JoinTeamCommand>
{
    private readonly IUserContextService _userContextService;

    public JoinTeamHandler(
        IUserContextService userContextService,
        ICacheService cacheService,
        IHubContext<DecryptoMessageHub> decryptoMessageHub)
        : base(cacheService, decryptoMessageHub)
    {
        _userContextService = userContextService;
    }

    public async Task Handle(JoinTeamCommand request, CancellationToken cancellationToken)
    {
        await MatchUpdate(request.matchId, async match =>
        {
            var userInfo = await _userContextService.GetUserInfo();
            
            if (match.Teams.SelectMany(t => t.Value.Players.Keys).Contains(userInfo.Id))
            {
                return;
            }

            match.Teams[request.team].Players.Add(userInfo.Id, userInfo.Name);

            await _decryptoMessageHub.Clients.Group(match.Id.ToString())
                .SendAsync(
                    "PlayerJoined",
                    new DecryptoPlayerEvent()
                    {
                        PlayerId = userInfo.Id,
                        PlayerName = userInfo.Name,
                        Team = request.team
                    },
                    cancellationToken);
            
            await SendSensitiveInfoAsync(_userContextService.GetId(), match, request.team, cancellationToken);

            await UpdateMatchState(match, DecryptMatchState.WaitingForPlayers);
        });
    }
}