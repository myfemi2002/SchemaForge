namespace SchemaForge.API.Models;

public sealed record ValidationIssueResponse(
    string Keyword,
    string Message,
    string? EvaluationPath,
    string? InstanceLocation,
    string? SchemaLocation);
