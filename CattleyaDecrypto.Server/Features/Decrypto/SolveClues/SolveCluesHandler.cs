using CattleyaDecrypto.Server.Models.Enums;
using CattleyaDecrypto.Server.Services.Interfaces;
using CattleyaDecrypto.Server.Services.SignalR;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CattleyaDecrypto.Server.Features.Decrypto.SolveClues;

public class SolveCluesHandler : DecryptoBaseHandler, IRequestHandler<SolveCluesCommand>
{
    private readonly IUserContextService _userContextService;

    public SolveCluesHandler(
        IUserContextService userContextService,
        ICacheService cacheService,
        IHubContext<DecryptoMessageHub> decryptoMessageHub)
        : base(cacheService, decryptoMessageHub)
    {
        _userContextService = userContextService;
    }

    public async Task Handle(SolveCluesCommand request, CancellationToken cancellationToken)
    {
        await MatchUpdate(request.MatchId, async match =>
        {
            if (match.State != DecryptMatchState.SolveClues)
            {
                throw new ApplicationException($"Wrong state for 'SolveClues' action - {match.State}");
            }

            AssertPlayerInTeam(match, request.Team, _userContextService.GetId());

            if (!match.RoundClues.TryGetValue(request.Team, out var cluesToSolve))
            {
                throw new ApplicationException("Could not solve unriddle clues");
            }

            match.RoundClues[request.Team].IsSolved = true;
            if (!cluesToSolve.Order.SequenceEqual(request.Order))
            {
                match.Teams[request.Team].MiscommunicationCount++;
            }

            await ScoreAndStateUpdateEvent(
                request.MatchId,
                DecryptMatchState.SolveClues,
                request.Team,
                match.Teams[request.Team].MiscommunicationCount,
                match.Teams[request.Team].InterceptionCount,
                cancellationToken);

            if (await UpdateMatchState(match, DecryptMatchState.SolveClues, cancellationToken))
            {
                await CluesUpdateEvent(match, cancellationToken);
            }
        });
    }
}