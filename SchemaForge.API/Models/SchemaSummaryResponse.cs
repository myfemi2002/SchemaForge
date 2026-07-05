namespace SchemaForge.API.Models;

public sealed record SchemaSummaryResponse(
    string Key,
    string Title,
    string Type,
    SchemaPropertySummaryResponse[] Properties);
