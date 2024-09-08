using CattleyaDecrypto.Server.Features.Decrypto;
using CattleyaDecrypto.Server.Features.Decrypto.AssignRiddler;
using CattleyaDecrypto.Server.Features.Decrypto.CreateNewMatch;
using CattleyaDecrypto.Server.Features.Decrypto.GetMatch;
using CattleyaDecrypto.Server.Features.Decrypto.Intercept;
using CattleyaDecrypto.Server.Features.Decrypto.JoinTeam;
using CattleyaDecrypto.Server.Features.Decrypto.SolveClues;
using CattleyaDecrypto.Server.Features.Decrypto.SubmitClues;
using CattleyaDecrypto.Server.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CattleyaDecrypto.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/decrypto")]
public class DecryptoMatchController : ControllerBase
{
    private readonly IMediator _mediator;

    public DecryptoMatchController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("create-match")]
    public Task<DecryptoMatchResponse> CreateMatch()
    {
        return _mediator.Send(new CreateNewMatchCommand());
    }
    
    [HttpGet("get-match")]
    public Task<DecryptoMatchResponse> GetMatch([FromQuery] Guid matchId)
    {
        return _mediator.Send(new GetMatchQuery(matchId));
    }
    
    [HttpPost("join-team")]
    public Task JoinTeam([FromQuery] Guid matchId, [FromQuery] DecryptoTeamEnum team)
    {
        return _mediator.Send(new JoinTeamCommand(matchId, team));
    }
    
    [HttpPost("assign-riddler")]
    public Task AssignRiddler([FromQuery] Guid matchId)
    {
        return _mediator.Send(new AssignRiddlerCommand(matchId));
    }

    [HttpPost("submit-clues")]
    public Task SubmitClues(SubmitCluesCommand model)
    {
        return _mediator.Send(model);
    }
    
    [HttpPost("solve-clues")]
    public Task SolveClues(SolveCluesCommand model)
    {
        return _mediator.Send(model);
    }
    
    [HttpPost("intercept")]
    public Task Intercept(InterceptCommand model)
    {
        return _mediator.Send(model);
    }
}