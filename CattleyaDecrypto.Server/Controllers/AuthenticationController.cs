using System.Security.Claims;
using CattleyaDecrypto.Server.Models.ViewModels;
using CattleyaDecrypto.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CattleyaDecrypto.Server.Controllers;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IUserContextService _userContextService;

    public AuthenticationController(IUserContextService userContextService)
    {
        _userContextService = userContextService;
    }

    [HttpPost("login")]
    public async Task<AuthInfoVm> Login(
        [FromQuery] string? name,
        [FromServices] INameGeneratorService nameGeneratorService)
    {
        name ??= nameGeneratorService.GenerateName();
        var userId = _userContextService.GetId() ?? Guid.NewGuid();
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Sid, userId.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

        return new AuthInfoVm()
        {
            UserId = userId,
            Name = name
        };
    }
    
    [Authorize]
    [HttpPost("changeName")]
    public async Task<AuthInfoVm> Login([FromQuery] string name)
    {
        var userId = _userContextService.GetId();
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Sid, userId.ToString()!)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));
        
        return new AuthInfoVm()
        {
            UserId = userId!.Value,
            Name = name
        };
    }
}