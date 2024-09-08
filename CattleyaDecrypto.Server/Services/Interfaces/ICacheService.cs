using RedLockNet;

namespace CattleyaDecrypto.Server.Services.Interfaces;

public interface ICacheService
{
    Task<T?> GetRecordAsync<T>(string recordId);
    Task SetRecordAsync<T>(string recordId, T instance);
    Task<IRedLock> LockRecord(string recordId, int expiryTime = 1000, int waitTime = 1000, int retryTime = 100);
    string GetKey<T>(string id);
    string GetKey<T>(Guid id);
}