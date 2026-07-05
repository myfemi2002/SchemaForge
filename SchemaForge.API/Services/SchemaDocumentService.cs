using System.Text.Json;
using Json.Schema;
using SchemaForge.API.Models;

namespace SchemaForge.API.Services;

public sealed class SchemaDocumentService
{
    public async Task<string> ReadSchemaJsonAsync(SchemaDefinition definition, CancellationToken cancellationToken = default)
    {
        using var stream = File.OpenRead(definition.SchemaPath);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync(cancellationToken);
    }

    public async Task<string> ReadSampleJsonAsync(SchemaDefinition definition, CancellationToken cancellationToken = default)
    {
        using var stream = File.OpenRead(definition.SamplePath);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync(cancellationToken);
    }

    public async Task<SchemaDocument> ReadSchemaDocumentAsync(SchemaDefinition definition, CancellationToken cancellationToken = default)
    {
        var schemaJson = await ReadSchemaJsonAsync(definition, cancellationToken);
        return JsonSerializer.Deserialize<SchemaDocument>(schemaJson)
            ?? throw new InvalidOperationException($"Unable to deserialize schema '{definition.Key}'.");
    }

    public async Task<JsonSchema> ReadCompiledSchemaAsync(SchemaDefinition definition, CancellationToken cancellationToken = default)
    {
        var schemaJson = await ReadSchemaJsonAsync(definition, cancellationToken);
        return JsonSchema.FromText(schemaJson);
    }
}
