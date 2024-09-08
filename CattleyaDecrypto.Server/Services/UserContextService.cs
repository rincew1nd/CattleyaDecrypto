using System.Security.Claims;
using CattleyaDecrypto.Server.Models.Models;
using CattleyaDecrypto.Server.Services.Interfaces;

namespace CattleyaDecrypto.Server.Services;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICacheService _cacheService;

    public UserContextService(
        IHttpContextAccessor httpContextAccessor,
        ICacheService cacheService)
    {
        _httpContextAccessor = httpContextAccessor;
        _cacheService = cacheService;
    }

    /// <summary>
    /// Get UserId from cookies.
    /// </summary>
    public Guid GetId()
    {
        if (Guid.TryParse(
                _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId))
        {
            return userId;
        }

        throw new UnauthorizedAccessException();
    }

    /// <summary>
    /// Get UserInfo from cache without throwing exception.
    /// </summary>
    public async Task<UserInfo?> TryGetUserInfo()
    {
        try
        {
            var userId = GetId();
            var userInfo = await _cacheService.GetRecordAsync<UserInfo>(_cacheService.GetKey<UserInfo>(userId));
            return userInfo!;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Get UserInfo from cache.
    /// </summary>
    public async Task<UserInfo> GetUserInfo()
    {
        var userId = GetId();
        var userInfo = await _cacheService.GetRecordAsync<UserInfo>(_cacheService.GetKey<UserInfo>(userId));
        if (userInfo == null)
        {
            throw new UnauthorizedAccessException();
        }
        return userInfo;
    }

    /// <summary>
    /// Save user to cache.
    /// </summary>
    public async Task SaveUserAsync(UserInfo userInfoNew)
    {
        await _cacheService.SetRecordAsync(_cacheService.GetKey<UserInfo>(userInfoNew.Id), userInfoNew);
    }
}