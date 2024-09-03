using System.Security.Claims;
using CattleyaDecrypto.Server.Services.Interfaces;

namespace CattleyaDecrypto.Server.Services;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetName()
    {
        return _httpContextAccessor.HttpContext?.User.Identity?.Name;
    }

    public Guid? GetId()
    {
        if (Guid.TryParse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Sid)?.Value, out var userId))
        {
            return userId;
        }
        return null;
    }
}