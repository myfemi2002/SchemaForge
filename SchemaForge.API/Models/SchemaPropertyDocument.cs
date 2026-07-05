namespace SchemaForge.API.Models;

public sealed record SchemaPropertyDocument(
    string Type,
    string? Description,
    int? MinLength,
    string? Format,
    int? Minimum,
    int? Maximum);
