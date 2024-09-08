using CattleyaDecrypto.Server.Models.Models;

namespace CattleyaDecrypto.Server.Services.Interfaces;

public interface IUserContextService
{
    Task<UserInfo?> TryGetUserInfo();
    Task<UserInfo> GetUserInfo();
    Guid GetId();
    Task SaveUserAsync(UserInfo userInfoNew);
}