using CattleyaDecrypto.Server.Models.Enums;
using MediatR;

namespace CattleyaDecrypto.Server.Features.Decrypto.JoinTeam;

public record JoinTeamCommand(Guid matchId, DecryptoTeamEnum team) : IRequest;