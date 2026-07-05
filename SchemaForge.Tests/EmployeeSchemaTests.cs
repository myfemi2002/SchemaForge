using System.Text.Json;
using System.Text.Json.Nodes;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
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

    private static readonly WebApplicationFactory<Program> Factory = new();

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

    [Fact]
    public async Task ValidationEndpoint_ShouldAcceptValidEmployeePayload()
    {
        using var client = Factory.CreateClient();
        using var content = JsonContent.Create(JsonNode.Parse(File.ReadAllText(SamplePath)));

        using var response = await client.PostAsync("/api/lessons/employee/validate", content);
        var responseJson = JsonNode.Parse(await response.Content.ReadAsStringAsync());

        response.EnsureSuccessStatusCode();
        Assert.True(responseJson?["isValid"]?.GetValue<bool>());
        Assert.Equal(0, responseJson?["errorCount"]?.GetValue<int>());
    }

    [Fact]
    public async Task ValidationEndpoint_ShouldReturnStructuredErrors_ForInvalidEmployeePayload()
    {
        using var client = Factory.CreateClient();
        using var content = JsonContent.Create(new
        {
            employeeId = "EMP001",
            firstName = "F",
            lastName = "Falana",
            age = 17,
            department = "Engineering",
            isActive = true
        });

        using var response = await client.PostAsync("/api/lessons/employee/validate", content);
        var responseJson = JsonNode.Parse(await response.Content.ReadAsStringAsync());
        var errors = responseJson?["errors"]?.AsArray();

        response.EnsureSuccessStatusCode();
        Assert.False(responseJson?["isValid"]?.GetValue<bool>());
        Assert.NotNull(errors);
        Assert.NotEmpty(errors!);
    }
}
