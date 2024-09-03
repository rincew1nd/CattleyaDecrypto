using CattleyaDecrypto.Server.Models.Enums;
using CattleyaDecrypto.Server.Models.Models;
using CattleyaDecrypto.Server.Models.ViewModels;
using CattleyaDecrypto.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CattleyaDecrypto.Server.Controllers;

[ApiController]
[Authorize]
[Route("decrypto")]
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
    public DecryptoMatch? GetMatch([FromQuery] Guid matchId)
    {
        return _decryptoMatchService.GetMatch(matchId);
    }
    
    [HttpPost("join-team")]
    public bool JoinTeam([FromQuery] Guid matchId, [FromQuery] TeamEnum team)
    {
        return _decryptoMatchService.JoinTeam(matchId, team, _userContextService.GetId()!.Value);
    }

    [HttpPost("give-clues")]
    public bool GiveClues(GiveCluesVm model)
    {
        return _decryptoMatchService.GiveClues(model);
    }
    
    [HttpPost("solve-clues")]
    public void SolveClues(SolveOrInterceptCluesVm model)
    {
        _decryptoMatchService.SolveClues(model);
    }
    
    [HttpPost("intercept")]
    public void Intercept(SolveOrInterceptCluesVm model)
    {
        _decryptoMatchService.Intercept(model);
    }
}