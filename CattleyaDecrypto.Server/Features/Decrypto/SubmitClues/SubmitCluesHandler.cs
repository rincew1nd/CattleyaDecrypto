﻿using CattleyaDecrypto.Server.Models.Enums;
using CattleyaDecrypto.Server.Models.EventModels;
using CattleyaDecrypto.Server.Services.Interfaces;
using CattleyaDecrypto.Server.Services.SignalR;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CattleyaDecrypto.Server.Features.Decrypto.SubmitClues;

public class SubmitCluesHandler : DecryptoBaseHandler, IRequestHandler<SubmitCluesCommand>
{
    private readonly IUserContextService _userContextService;

    public SubmitCluesHandler(
        IUserContextService userContextService,
        ICacheService cacheService,
        IHubContext<DecryptoMessageHub> decryptoMessageHub)
        : base(cacheService, decryptoMessageHub)
    {
        _userContextService = userContextService;
    }

    public async Task Handle(SubmitCluesCommand request, CancellationToken cancellationToken)
    {
        await MatchUpdate(request.MatchId, async match =>
        {
            if (match.State is not DecryptMatchState.GiveClues)
            {
                throw new ApplicationException($"Wrong state for 'Solve clues' action - {match.State}");
            }

            AssertPlayerInTeam(match, request.Team, _userContextService.GetId());

            if (match.RoundClues.ContainsKey(request.Team) &&
                (match.RoundClues[request.Team].Clues?.Any() ?? false))
            {
                throw new ApplicationException(
                    $"Clues are already been given - {match.RoundClues[request.Team].Clues}");
            }

            match.RoundClues[request.Team].Clues = request.Clues;

            await _decryptoMessageHub.Clients
                .Group(match.Id.ToString())
                .SendAsync(
                    "SolveClues",
                    new PrepareClueEvent()
                    {
                        Team = request.Team,
                        Clues = request.Clues
                    },
                    cancellationToken);

            await UpdateMatchState(match, DecryptMatchState.GiveClues, cancellationToken);
        });
    }
}