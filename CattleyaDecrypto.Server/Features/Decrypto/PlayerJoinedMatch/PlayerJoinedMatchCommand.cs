using MediatR;

namespace CattleyaDecrypto.Server.Features.Decrypto.PlayerJoinedMatch;

public record PlayerJoinedMatchCommand(Guid MatchId) : IRequest;