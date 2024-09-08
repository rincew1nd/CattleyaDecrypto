using MediatR;

namespace CattleyaDecrypto.Server.Features.Decrypto.AssignRiddler;

public record AssignRiddlerCommand(Guid MatchId) : IRequest;