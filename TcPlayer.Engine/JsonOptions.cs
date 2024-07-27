using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TcPlayer.Engine;
public static class JsonOptions
{
    public static JsonSerializerOptions ForDiskStorage = new JsonSerializerOptions
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.Strict,
        IncludeFields = false,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = 
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
        }
    };
}
