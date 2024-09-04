using System.Text.Json;

namespace CattleyaDecrypto.Server.Architecture;

public static class Extensions
{
    public static T? CloneJson<T>(this T source)
    {
        if (ReferenceEquals(source, null)) return default;
        var jsonString = JsonSerializer.Serialize(source);
        return JsonSerializer.Deserialize<T>(jsonString);
    }
}