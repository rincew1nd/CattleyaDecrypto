using CattleyaDecrypto.Server.Models.Enums;
using MediatR;

namespace CattleyaDecrypto.Server.Features.Decrypto.Intercept;

public record InterceptCommand(Guid MatchId, DecryptoTeamEnum Team, int[] Order) : IRequest;