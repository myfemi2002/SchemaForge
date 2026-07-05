using System.Text.Json;
using SchemaForge.API.Models;
using SchemaForge.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<SchemaCatalogService>();
builder.Services.AddSingleton<SchemaDocumentService>();
builder.Services.AddSingleton<SchemaValidationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

app.MapGet("/api/schemas", (SchemaCatalogService catalogService) =>
{
    var schemas = catalogService.GetAll()
        .Select(schema => new SchemaCatalogItemResponse(
            schema.Key,
            schema.Title,
            schema.Description,
            Path.GetFileName(schema.SchemaPath),
            Path.GetFileName(schema.SamplePath)))
        .ToArray();

    return Results.Ok(schemas);
})
    .WithName("GetSchemas")
    .WithSummary("Gets the schema catalog currently available in SchemaForge.");

app.MapGet("/api/schemas/{key}", async (string key, SchemaCatalogService catalogService, SchemaDocumentService documentService) =>
{
    var definition = ResolveSchema(catalogService, key);
    var schema = await documentService.ReadSchemaDocumentAsync(definition);

    var propertySummaries = schema.Properties
        .Select(property => new SchemaPropertySummaryResponse(
            property.Key,
            property.Value.Type,
            schema.Required.Contains(property.Key),
            property.Value.Description,
            property.Value.MinLength,
            property.Value.Format,
            property.Value.Minimum,
            property.Value.Maximum))
        .ToArray();

    return Results.Ok(new SchemaSummaryResponse(
        definition.Key,
        schema.Title,
        schema.Type,
        propertySummaries));
})
    .WithName("GetSchemaSummary")
    .WithSummary("Gets a schema summary from the schema catalog.");

app.MapGet("/api/schemas/{key}/document", async (string key, SchemaCatalogService catalogService, SchemaDocumentService documentService) =>
{
    var definition = ResolveSchema(catalogService, key);
    var schemaJson = await documentService.ReadSchemaJsonAsync(definition);

    return Results.Text(schemaJson, "application/schema+json");
})
    .WithName("GetSchemaDocument")
    .WithSummary("Gets the raw JSON Schema document for a catalog schema.");

app.MapGet("/api/schemas/{key}/sample", async (string key, SchemaCatalogService catalogService, SchemaDocumentService documentService) =>
{
    var definition = ResolveSchema(catalogService, key);
    var sampleJson = await documentService.ReadSampleJsonAsync(definition);

    return Results.Text(sampleJson, "application/json");
})
    .WithName("GetSchemaSample")
    .WithSummary("Gets the sample JSON payload for a catalog schema.");

app.MapPost("/api/schemas/{key}/validate", async (string key, JsonElement payload, SchemaCatalogService catalogService, SchemaDocumentService documentService, SchemaValidationService validationService) =>
{
    var definition = ResolveSchema(catalogService, key);
    var schema = await documentService.ReadCompiledSchemaAsync(definition);
    var report = validationService.Validate(definition, schema, payload);

    return Results.Ok(report);
})
    .WithName("ValidateSchemaPayload")
    .WithSummary("Validates a JSON payload against a named schema in the catalog.");

app.MapGet("/api/lessons/employee/sample", async (SchemaCatalogService catalogService, SchemaDocumentService documentService) =>
{
    var definition = catalogService.GetByKey("employee");
    var sampleJson = await documentService.ReadSampleJsonAsync(definition);
    return Results.Text(sampleJson, "application/json");
})
    .WithName("GetEmployeeSample")
    .WithSummary("Gets the sample employee JSON document used in lesson 1.");

app.MapGet("/api/lessons/employee/schema", async (SchemaCatalogService catalogService, SchemaDocumentService documentService) =>
{
    var definition = catalogService.GetByKey("employee");
    var schemaJson = await documentService.ReadSchemaJsonAsync(definition);
    return Results.Text(schemaJson, "application/schema+json");
})
    .WithName("GetEmployeeSchema")
    .WithSummary("Gets the manual employee JSON Schema created in lesson 1.");

app.MapGet("/api/lessons/employee/schema/summary", async (SchemaCatalogService catalogService, SchemaDocumentService documentService) =>
{
    var definition = catalogService.GetByKey("employee");
    var schema = await documentService.ReadSchemaDocumentAsync(definition);

    var propertySummaries = schema.Properties
        .Select(property => new SchemaPropertySummaryResponse(
            property.Key,
            property.Value.Type,
            schema.Required.Contains(property.Key),
            property.Value.Description,
            property.Value.MinLength,
            property.Value.Format,
            property.Value.Minimum,
            property.Value.Maximum))
        .ToArray();

    return Results.Ok(new SchemaSummaryResponse(
        definition.Key,
        schema.Title,
        schema.Type,
        propertySummaries));
})
    .WithName("GetEmployeeSchemaSummary")
    .WithSummary("Explains the employee schema in an API-friendly shape.");

app.MapPost("/api/lessons/employee/validate", async (JsonElement payload, SchemaCatalogService catalogService, SchemaDocumentService documentService, SchemaValidationService validationService) =>
{
    var definition = catalogService.GetByKey("employee");
    var schema = await documentService.ReadCompiledSchemaAsync(definition);
    var report = validationService.Validate(definition, schema, payload);

    return Results.Ok(report);
})
    .WithName("ValidateEmployeePayload")
    .WithSummary("Validates an employee JSON payload and returns a structured validation report.");

SchemaDefinition ResolveSchema(SchemaCatalogService catalogService, string key)
{
    try
    {
        return catalogService.GetByKey(key);
    }
    catch (KeyNotFoundException)
    {
        throw new BadHttpRequestException($"Schema '{key}' was not found.", statusCode: StatusCodes.Status404NotFound);
    }
}

app.Run();

public partial class Program;
