using BusinessLayer.Interface;
using StackExchange.Redis;
using System.Text.Json;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _cache;
    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _cache = redis.GetDatabase();
    }

    public void SetData<T>(string key, T value, TimeSpan expiration)
    {
        _cache.StringSet(key, JsonSerializer.Serialize(value), expiration);
    }

    public T GetData<T>(string key)
    {
        string data = _cache.StringGet(key);
        return string.IsNullOrEmpty(data) ? default : JsonSerializer.Deserialize<T>(data);
    }

    public void RemoveData(string key)
    {
        _cache.KeyDelete(key);
    }
}
