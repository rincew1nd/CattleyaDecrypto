using CattleyaDecrypto.Server.Models.Enums;
using MediatR;

namespace CattleyaDecrypto.Server.Features.Decrypto.SolveClues;

public record SolveCluesCommand(Guid MatchId, DecryptoTeamEnum Team, int[] Order) : IRequest;