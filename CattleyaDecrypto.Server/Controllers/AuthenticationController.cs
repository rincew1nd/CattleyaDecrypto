using System.Security.Claims;
using CattleyaDecrypto.Server.Models.Models;
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
    public async Task<UserInfo> Login(
        [FromQuery] string? name,
        [FromServices] INameGeneratorService nameGeneratorService)
    {
        var userInfo = await _userContextService.TryGetUserInfo();
        var userId = userInfo?.Id ?? Guid.NewGuid();
        var userName = name ?? userInfo?.Name ?? nameGeneratorService.GenerateName();
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userName),
            new(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

        var userInfoNew = new UserInfo { Id = userId, Name = userName };
        await _userContextService.SaveUserAsync(userInfoNew);
        return userInfoNew;
    }
    
}