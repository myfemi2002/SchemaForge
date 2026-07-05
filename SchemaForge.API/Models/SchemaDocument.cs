namespace SchemaForge.API.Models;

public sealed record SchemaDocument(
    string Title,
    string Type,
    Dictionary<string, SchemaPropertyDocument> Properties,
    string[] Required);
