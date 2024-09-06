using CattleyaDecrypto.Server.Models.Enums;
using CattleyaDecrypto.Server.Models.Models;
using CattleyaDecrypto.Server.Models.ViewModels;
using CattleyaDecrypto.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CattleyaDecrypto.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/decrypto")]
public class DecryptoMatchController : ControllerBase
{
    private readonly IUserContextService _userContextService;
    private readonly IDecryptoMatchService _decryptoMatchService;

    public DecryptoMatchController(
        IUserContextService userContextService,
        IDecryptoMatchService decryptoMatchService)
    {
        _userContextService = userContextService;
        _decryptoMatchService = decryptoMatchService;
    }
    
    [HttpPost("create-match")]
    public DecryptoMatch CreateMatch()
    {
        return _decryptoMatchService.CreateMatch();
    }
    
    [HttpGet("get-match")]
    public DecryptoMatch GetMatch([FromQuery] Guid matchId)
    {
        return _decryptoMatchService.GetMatch(matchId, _userContextService.GetId()!.Value);
    }
    
    [HttpPost("join-team")]
    public async Task<bool> JoinTeam([FromQuery] Guid matchId, [FromQuery] TeamEnum team)
    {
        return await _decryptoMatchService.JoinTeamAsync(
            matchId,
            team,
            _userContextService.GetId()!.Value,
            _userContextService.GetName()!);
    }
    
    [HttpPost("assign-riddler")]
    public async Task<bool> AssignRiddler([FromQuery] Guid matchId)
    {
        return await _decryptoMatchService.AssignRiddlerAsync(
            matchId,
            _userContextService.GetId()!.Value,
            _userContextService.GetName()!);
    }

    [HttpPost("submit-clues")]
    public async Task SubmitClues(SubmitCluesVm model)
    {
        await _decryptoMatchService.SubmitCluesAsync(model, _userContextService.GetId()!.Value);
    }
    
    [HttpPost("solve-clues")]
    public async Task SolveClues(SolveOrInterceptCluesVm model)
    {
        await _decryptoMatchService.SolveCluesASync(model, _userContextService.GetId()!.Value);
    }
    
    [HttpPost("intercept")]
    public async Task Intercept(SolveOrInterceptCluesVm model)
    {
        await _decryptoMatchService.InterceptASync(model, _userContextService.GetId()!.Value);
    }
}