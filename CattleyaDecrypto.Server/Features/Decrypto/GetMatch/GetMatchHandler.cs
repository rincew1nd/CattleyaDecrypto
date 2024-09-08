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
        return new DecryptoMatchResponse(match);
    }
}