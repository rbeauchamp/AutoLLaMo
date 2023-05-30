using System.Text.Json;
using System.Text.Json.Serialization;
using NJsonSchema;
using NJsonSchema.Generation;

namespace AutoLLaMo.Plugins;

public static class JsonExtensions
{
    public static readonly JsonSchemaGeneratorSettings Settings = new()
    {
        FlattenInheritanceHierarchy = true,
        SerializerSettings = null,
        SerializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        },
    };

    /// <summary>
    /// A converter that can serialize an abstract Output type.
    /// </summary>
    private sealed class OutputPolymorphicSerializerConverter : JsonConverter<Output>
    {
        public override Output Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException($"Deserializing not supported. Type={typeToConvert}.");
        }

        public override void Write(Utf8JsonWriter writer, Output value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }

    private static JsonSerializerOptions GetSerializerOptions(bool writeIndented) => new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = writeIndented,
        Converters =
        {
            new OutputPolymorphicSerializerConverter(),
        },
    };

    public static string? ToJson(this object? value, bool writeIndented = false)
    {
        return value == null ? null : JsonSerializer.Serialize(value, GetSerializerOptions(writeIndented));
    }

    /// <summary>
    /// Gets the JsonSchema for the command.
    /// </summary>
    public static JsonSchema ToJsonSchema(this Type type)
    {
        return JsonSchema.FromType(type, Settings);
    }
}
