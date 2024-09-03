using System.Security.Claims;
using CattleyaDecrypto.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CattleyaDecrypto.Server.Controllers;

[ApiController]
[Route("authentication")]
public class AuthenticationController : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromQuery] string? name,
        [FromServices] INameGeneratorService nameGeneratorService)
    {
        name ??= nameGeneratorService.GenerateName();
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Sid, Guid.NewGuid().ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

        return Ok();
    }
    
    [Authorize]
    [HttpPost("changeName")]
    public async Task<IActionResult> Login(
        [FromQuery] string name,
        [FromServices] IUserContextService userContextService)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Sid, userContextService.GetId().ToString()!)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

        return Ok();
    }
}