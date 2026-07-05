using System.Text.Json.Nodes;

namespace SchemaForge.API.Models;

public sealed record ValidationReportResponse(
    string SchemaKey,
    string SchemaTitle,
    DateTime EvaluatedAtUtc,
    bool IsValid,
    int ErrorCount,
    ValidationIssueResponse[] Errors,
    JsonObject? Report);
