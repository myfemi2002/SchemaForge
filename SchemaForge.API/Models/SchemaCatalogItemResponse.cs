namespace SchemaForge.API.Models;

public sealed record SchemaCatalogItemResponse(
    string Key,
    string Title,
    string Description,
    string SchemaPath,
    string SamplePath);
