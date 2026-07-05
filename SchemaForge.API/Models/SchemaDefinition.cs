namespace SchemaForge.API.Models;

public sealed record SchemaDefinition(
    string Key,
    string Title,
    string Description,
    string SchemaPath,
    string SamplePath);
