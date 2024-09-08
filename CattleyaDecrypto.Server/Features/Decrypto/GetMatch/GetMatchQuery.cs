using MediatR;

namespace CattleyaDecrypto.Server.Features.Decrypto.GetMatch;

public record GetMatchQuery(Guid MatchId) : IRequest<DecryptoMatchResponse>;