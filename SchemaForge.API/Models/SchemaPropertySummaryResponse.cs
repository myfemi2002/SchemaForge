namespace SchemaForge.API.Models;

public sealed record SchemaPropertySummaryResponse(
    string Name,
    string Type,
    bool IsRequired,
    string? Description,
    int? MinLength,
    string? Format,
    int? Minimum,
    int? Maximum);
