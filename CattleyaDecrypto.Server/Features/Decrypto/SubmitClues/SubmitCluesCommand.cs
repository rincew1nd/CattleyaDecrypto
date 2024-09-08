using CattleyaDecrypto.Server.Models.Enums;
using MediatR;

namespace CattleyaDecrypto.Server.Features.Decrypto.SubmitClues;

public record SubmitCluesCommand(Guid MatchId, DecryptoTeamEnum Team, string[] Clues) : IRequest;