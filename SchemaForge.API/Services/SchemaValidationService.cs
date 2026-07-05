using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Schema;
using SchemaForge.API.Models;

namespace SchemaForge.API.Services;

public sealed class SchemaValidationService
{
    public ValidationReportResponse Validate(SchemaDefinition definition, JsonSchema schema, JsonElement payload)
    {
        var evaluation = schema.Evaluate(payload);
        var reportNode = JsonSerializer.SerializeToNode(evaluation) as JsonObject;
        var errors = new List<ValidationIssueResponse>();

        CollectErrors(reportNode, errors);

        if (!evaluation.IsValid && errors.Count == 0)
        {
            errors.Add(new ValidationIssueResponse(
                Keyword: "schema",
                Message: "Payload failed JSON Schema validation.",
                EvaluationPath: reportNode?["evaluationPath"]?.GetValue<string>(),
                InstanceLocation: reportNode?["instanceLocation"]?.GetValue<string>(),
                SchemaLocation: reportNode?["schemaLocation"]?.GetValue<string>()));
        }

        return new ValidationReportResponse(
            SchemaKey: definition.Key,
            SchemaTitle: definition.Title,
            EvaluatedAtUtc: DateTime.UtcNow,
            IsValid: evaluation.IsValid,
            ErrorCount: errors.Count,
            Errors: errors.ToArray(),
            Report: reportNode);
    }

    private static void CollectErrors(JsonObject? reportNode, List<ValidationIssueResponse> errors)
    {
        if (reportNode is null)
        {
            return;
        }

        if (reportNode["errors"] is JsonObject errorObject)
        {
            foreach (var entry in errorObject)
            {
                errors.Add(new ValidationIssueResponse(
                    Keyword: entry.Key,
                    Message: entry.Value?.GetValue<string>() ?? "Validation failed.",
                    EvaluationPath: reportNode["evaluationPath"]?.GetValue<string>(),
                    InstanceLocation: reportNode["instanceLocation"]?.GetValue<string>(),
                    SchemaLocation: reportNode["schemaLocation"]?.GetValue<string>()));
            }
        }

        if (reportNode["details"] is JsonArray details)
        {
            foreach (var child in details.OfType<JsonObject>())
            {
                CollectErrors(child, errors);
            }
        }
    }
}
