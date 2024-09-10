using CattleyaDecrypto.Server.Models.Enums;
using CattleyaDecrypto.Server.Models.EventModels;
using CattleyaDecrypto.Server.Services.Interfaces;
using CattleyaDecrypto.Server.Services.SignalR;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CattleyaDecrypto.Server.Features.Decrypto.Intercept;

public class InterceptHandler : DecryptoBaseHandler, IRequestHandler<InterceptCommand>
{
    private readonly IUserContextService _userContextService;

    public InterceptHandler(
        IUserContextService userContextService,
        ICacheService cacheService,
        IHubContext<DecryptoMessageHub> decryptoMessageHub)
        : base(cacheService, decryptoMessageHub)
    {
        _userContextService = userContextService;
    }

    public async Task Handle(InterceptCommand request, CancellationToken cancellationToken)
    {
        await MatchUpdate(request.MatchId, async match =>
        {
            if (match.State != DecryptMatchState.Intercept)
            {
                throw new ApplicationException($"Wrong state for 'Intercept' action - {match.State}");
            }

            var oppositeTeam = OppositeTeam(request.Team);
            if (oppositeTeam == DecryptoTeamEnum.Unknown)
            {
                throw new ApplicationException("Player is not in a team");
            }

            AssertPlayerInTeam(match, request.Team, _userContextService.GetId());

            if (!match.RoundClues.TryGetValue(oppositeTeam, out var cluesToSolve))
            {
                throw new ApplicationException("Could not solve unriddle clues");
            }

            match.RoundClues[oppositeTeam].IsIntercepted = true;
            if (cluesToSolve.Order.SequenceEqual(request.Order))
            {
                match.Teams[request.Team].InterceptionCount++;
            }

            await ScoreAndStateUpdateEvent(
                request.MatchId,
                DecryptMatchState.Intercept,
                request.Team,
                match.Teams[request.Team].MiscommunicationCount,
                match.Teams[request.Team].InterceptionCount,
                cancellationToken);
            
            await UpdateMatchState(match, DecryptMatchState.Intercept, cancellationToken);
            
            if (match.State == DecryptMatchState.Finished)
            {
                await _decryptoMessageHub.Clients
                    .Group(match.Id.ToString())
                    .SendAsync(
                        "MatchFinished",
                        new MatchFinishedEvent
                        {
                            Team = match.WonTeam,
                            Words = match.Teams[oppositeTeam].Words
                        },
                        cancellationToken);
            }
        });
    }
}