using MediatR;

namespace CattleyaDecrypto.Server.Features.Decrypto.CreateNewMatch;

public record CreateNewMatchCommand : IRequest<DecryptoMatchResponse>;