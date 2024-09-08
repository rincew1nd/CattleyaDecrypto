using System.Text.Json;
using CattleyaDecrypto.Server.Architecture;
using CattleyaDecrypto.Server.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace CattleyaDecrypto.Server.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly RedLockFactory _redLockFactory;

    private JsonSerializerOptions _serializerOptions = new()
    {
        Converters = { new JsonDictionaryTKeyEnumTValueConverter() }
    };

    public CacheService(IDistributedCache cache, IConfiguration configuration)
    {
        _cache = cache;
        _redLockFactory = RedLockFactory.Create(new List<RedLockMultiplexer>()
        {
            ConnectionMultiplexer.Connect(configuration.GetSection("Redis").Value!)
        });
    }

    public async Task<T?> GetRecordAsync<T>(string recordId)
    {
        var record = await _cache.GetStringAsync(recordId);
        return record != null ? JsonSerializer.Deserialize<T>(record, _serializerOptions) : default;
    }

    public async Task SetRecordAsync<T>(string recordId, T instance)
    {
        // TODO record lifetime
        var serializedInstance = JsonSerializer.Serialize(instance, _serializerOptions);
        await _cache.SetStringAsync(recordId, serializedInstance);
    }

    public async Task<IRedLock> LockRecord(
        string recordId, int expiryTime = 1000, int waitTime = 1000, int retryTime = 100)
    {
        var redLock = await _redLockFactory.CreateLockAsync(
            recordId,
            TimeSpan.FromMilliseconds(expiryTime),
            TimeSpan.FromMilliseconds(waitTime),
            TimeSpan.FromMilliseconds(retryTime)
        );
        if (redLock.IsAcquired)
        {
            return redLock;
        }

        throw new TimeoutException($"Failed to lock a record {recordId}");
    }

    public string GetKey<T>(string id) => $"{typeof(T).Name}-{id}";
    public string GetKey<T>(Guid id) => GetKey<T>(id.ToString());
}