using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Persistence.Json;

internal static class JsonPersistenceOptions
{
    internal static readonly JsonSerializerOptions Default = BuildOptions();

    private static JsonSerializerOptions BuildOptions()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
        options.MakeReadOnly(populateMissingResolver: true);
        return options;
    }
}
