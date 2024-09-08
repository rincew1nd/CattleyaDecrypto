using CattleyaDecrypto.Server.Models.Enums;
using CattleyaDecrypto.Server.Models.EventModels;
using CattleyaDecrypto.Server.Models.Models;
using CattleyaDecrypto.Server.Services.Interfaces;
using CattleyaDecrypto.Server.Services.SignalR;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CattleyaDecrypto.Server.Features.Decrypto.AssignRiddler;

public class AssignRiddlerHandler : DecryptoBaseHandler, IRequestHandler<AssignRiddlerCommand>
{
    private readonly IUserContextService _userContextService;
    private readonly IOrderGeneratorService _orderGeneratorService;

    public AssignRiddlerHandler(
        IUserContextService userContextService,
        IOrderGeneratorService orderGeneratorService,
        ICacheService cacheService,
        IHubContext<DecryptoMessageHub> decryptoMessageHub)
        : base(cacheService, decryptoMessageHub)
    {
        _userContextService = userContextService;
        _orderGeneratorService = orderGeneratorService;
    }

    public async Task Handle(AssignRiddlerCommand request, CancellationToken cancellationToken)
    {
        await MatchUpdate(request.MatchId, async match =>
        {
            if (match.State != DecryptMatchState.GiveClues)
            {
                throw new ApplicationException($"Wrong state for 'Give clues' action - {match.State}");
            }

            var userInfo = await _userContextService.GetUserInfo();
            
            var playerTeam = GetPlayerTeam(match, userInfo.Id);
            if (playerTeam == default)
            {
                throw new ApplicationException("Player is not in a team");
            }

            if (match.RoundClues.ContainsKey(playerTeam))
            {
                throw new ApplicationException("Riddler is already assigned!");
            }

            var order = _orderGeneratorService.GetRandomOrder(ref match.AvailableWordOrders);
            var result = match.RoundClues.TryAdd(
                playerTeam,
                new CluesToSolve
                {
                    Order = order,
                    RiddlerId = userInfo.Id
                });

            if (result)
            {
                await _decryptoMessageHub.Clients
                    .Group(match.Id.ToString())
                    .SendAsync(
                        "AssignRiddler",
                        new DecryptoPlayerEvent()
                        {
                            PlayerId = userInfo.Id,
                            PlayerName = userInfo.Name,
                            Team = playerTeam
                        },
                        cancellationToken);

                await SendSensitiveInfoAsync(_userContextService.GetId(), match, playerTeam, cancellationToken);
                
                await UpdateMatchState(match, DecryptMatchState.GiveClues);
            }
        });
    }
}