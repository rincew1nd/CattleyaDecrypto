using CattleyaDecrypto.Server.Features.Decrypto.CreateNewMatch;
using CattleyaDecrypto.Server.Services.Interfaces;
using CattleyaDecrypto.Server.Services.SignalR;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CattleyaDecrypto.Server.Features.Decrypto.GetMatch;

public class GetMatchHandler : DecryptoBaseHandler, IRequestHandler<GetMatchQuery, DecryptoMatchResponse>
{
    public GetMatchHandler(
        ICacheService cacheService,
        IHubContext<DecryptoMessageHub> decryptoMessageHub)
        : base(cacheService, decryptoMessageHub)
    {
    }
    
    public async Task<DecryptoMatchResponse> Handle(GetMatchQuery request, CancellationToken cancellationToken)
    {
        var match = await GetMatchAsync(request.MatchId);

        var response = new DecryptoMatchResponse()
        {
            Id = match.Id,
            Round = match.Round,
            State = match.State,
            Teams = new Dictionary<int, CreateNewMatchTeamResponse>()
        };
        foreach (var (key, value) in match.Teams)
        {
            response.Teams.Add((int)key, new CreateNewMatchTeamResponse()
            {
                InterceptionCount = value.InterceptionCount,
                MiscommunicationCount = value.MiscommunicationCount,
                Players = value.Players
            });
        }
        
        return response;
    }
}