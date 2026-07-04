using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var schemaFilePath = Path.Combine(app.Environment.ContentRootPath, "Schemas", "employee.schema.json");
var sampleFilePath = Path.Combine(app.Environment.ContentRootPath, "Samples", "employee.sample.json");

app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

app.MapGet("/api/lessons/employee/sample", async () =>
{
    var sampleJson = await File.ReadAllTextAsync(sampleFilePath);
    return Results.Text(sampleJson, "application/json");
})
    .WithName("GetEmployeeSample")
    .WithSummary("Gets the sample employee JSON document used in lesson 1.");

app.MapGet("/api/lessons/employee/schema", async () =>
{
    var schemaJson = await File.ReadAllTextAsync(schemaFilePath);
    return Results.Text(schemaJson, "application/schema+json");
})
    .WithName("GetEmployeeSchema")
    .WithSummary("Gets the manual employee JSON Schema created in lesson 1.");

app.MapGet("/api/lessons/employee/schema/summary", async () =>
{
    var schemaJson = await File.ReadAllTextAsync(schemaFilePath);
    var schema = JsonSerializer.Deserialize<EmployeeSchemaDocument>(schemaJson)
        ?? throw new InvalidOperationException("Unable to read the employee schema document.");

    var propertySummaries = schema.Properties
        .Select(property => new EmployeeSchemaPropertySummary(
            property.Key,
            property.Value.Type,
            schema.Required.Contains(property.Key),
            property.Value.Description,
            property.Value.MinLength,
            property.Value.Format,
            property.Value.Minimum,
            property.Value.Maximum))
        .ToArray();

    return Results.Ok(new EmployeeSchemaSummaryResponse(
        schema.Title,
        schema.Type,
        propertySummaries));
})
    .WithName("GetEmployeeSchemaSummary")
    .WithSummary("Explains the employee schema in an API-friendly shape.");

app.Run();

internal sealed record EmployeeSchemaSummaryResponse(
    string Title,
    string Type,
    EmployeeSchemaPropertySummary[] Properties);

internal sealed record EmployeeSchemaPropertySummary(
    string Name,
    string Type,
    bool IsRequired,
    string? Description,
    int? MinLength,
    string? Format,
    int? Minimum,
    int? Maximum);

internal sealed record EmployeeSchemaDocument(
    string Title,
    string Type,
    Dictionary<string, EmployeeSchemaProperty> Properties,
    string[] Required);

internal sealed record EmployeeSchemaProperty(
    string Type,
    string? Description,
    int? MinLength,
    string? Format,
    int? Minimum,
    int? Maximum);
