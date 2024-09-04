﻿using System.Globalization;
using CattleyaDecrypto.Server.Models.Enums;
using CattleyaDecrypto.Server.Models.Models;
using CattleyaDecrypto.Server.Models.ViewModels;
using CattleyaDecrypto.Server.Services.Interfaces;
using CattleyaDecrypto.Server.Services.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

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

    [HttpGet("signalr-test")]
    public async Task SignalRTest([FromServices] IHubContext<DecryptoMessageHub> hub)
    {
        await hub.Clients.All.SendAsync("SignalRTest", DateTime.Now.ToString(CultureInfo.InvariantCulture));
    }

    [HttpGet("send-match")]
    public async Task SendMatchUpdate(Guid matchId)
    {
        await _decryptoMatchService.SendMatchUpdate(matchId);
    }

    [HttpGet("words")]
    public IEnumerable<string> Get(
        [FromServices] IWordPuzzleService wordPuzzleService,
        [FromQuery] int wordCount)
    {
        return wordPuzzleService.PuzzleWords(wordCount);
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

    [HttpPost("give-clues")]
    public async Task<bool> GiveClues(GiveCluesVm model)
    {
        return await _decryptoMatchService.GiveCluesAsync(model, _userContextService.GetId()!.Value);
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