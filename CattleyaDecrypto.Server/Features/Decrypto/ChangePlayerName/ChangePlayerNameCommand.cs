using MediatR;

namespace CattleyaDecrypto.Server.Features.Decrypto.ChangePlayerName;

public record ChangePlayerNameCommand(Guid MatchId) : IRequest;