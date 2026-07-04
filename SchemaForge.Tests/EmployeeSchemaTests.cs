using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Schema;
using Xunit;

namespace SchemaForge.Tests;

public sealed class EmployeeSchemaTests
{
    private static readonly string RepositoryRoot = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    private static readonly string SchemaPath = Path.Combine(
        RepositoryRoot,
        "SchemaForge.API",
        "Schemas",
        "employee.schema.json");

    private static readonly string SamplePath = Path.Combine(
        RepositoryRoot,
        "SchemaForge.API",
        "Samples",
        "employee.sample.json");

    [Fact]
    public void EmployeeSample_ShouldMatchEmployeeSchema()
    {
        var schema = JsonSchema.FromText(File.ReadAllText(SchemaPath));
        using var sampleDocument = JsonDocument.Parse(File.ReadAllText(SamplePath));

        var results = schema.Evaluate(sampleDocument.RootElement);

        Assert.True(results.IsValid);
    }

    [Fact]
    public void EmployeeSchema_ShouldRequireEmail()
    {
        var schemaDocument = JsonNode.Parse(File.ReadAllText(SchemaPath));
        var required = schemaDocument?["required"]?.AsArray().Select(node => node?.GetValue<string>()).ToArray();

        Assert.NotNull(required);
        Assert.Contains("email", required!);
    }

    [Fact]
    public void InvalidEmployeePayload_ShouldFailValidation()
    {
        var schema = JsonSchema.FromText(File.ReadAllText(SchemaPath));
        using var invalidPayloadDocument = JsonDocument.Parse("""
        {
          "employeeId": "EMP001",
          "firstName": "F",
          "lastName": "Falana",
          "age": 17,
          "department": "Engineering",
          "isActive": true
        }
        """);

        var results = schema.Evaluate(invalidPayloadDocument.RootElement);

        Assert.False(results.IsValid);
    }
}
