using CattleyaDecrypto.Server.Models.Enums;
using CattleyaDecrypto.Server.Models.Models;
using CattleyaDecrypto.Server.Models.ViewModels;

namespace CattleyaDecrypto.Server.Services.Interfaces;

public interface IDecryptoMatchService
{
    DecryptoMatch CreateMatch();
    DecryptoMatch? GetMatch(Guid matchId);
    bool JoinTeam(Guid matchId, TeamEnum team, Guid userId);
    bool GiveClues(GiveCluesVm model);
    void SolveClues(SolveOrInterceptCluesVm model);
    void Intercept(SolveOrInterceptCluesVm model);
}