using CattleyaDecrypto.Server.Models.Enums;
using CattleyaDecrypto.Server.Models.Models;
using CattleyaDecrypto.Server.Models.ViewModels;

namespace CattleyaDecrypto.Server.Services.Interfaces;

public interface IDecryptoMatchService
{
    DecryptoMatch CreateMatch();
    DecryptoMatch GetMatch(Guid matchId, Guid userId);
    Task<bool> JoinTeamAsync(Guid matchId, TeamEnum team, Guid userId, string name);
    Task<bool> AssignRiddlerAsync(Guid matchId, Guid userId, string userName);
    Task SubmitCluesAsync(SubmitCluesVm model, Guid userId);
    Task SolveCluesASync(SolveOrInterceptCluesVm model, Guid userId);
    Task InterceptASync(SolveOrInterceptCluesVm model, Guid userId);
}