using SchemaForge.API.Models;

namespace SchemaForge.API.Services;

public sealed class SchemaCatalogService
{
    private readonly string _contentRootPath;

    public SchemaCatalogService(IHostEnvironment hostEnvironment)
    {
        _contentRootPath = hostEnvironment.ContentRootPath;
    }

    public IReadOnlyCollection<SchemaDefinition> GetAll()
    {
        return
        [
            new SchemaDefinition(
                Key: "employee",
                Title: "Employee",
                Description: "Manual employee JSON Schema used in the first validation lessons.",
                SchemaPath: Path.Combine(_contentRootPath, "Schemas", "employee.schema.json"),
                SamplePath: Path.Combine(_contentRootPath, "Samples", "employee.sample.json"))
        ];
    }

    public SchemaDefinition GetByKey(string key)
    {
        var definition = GetAll().FirstOrDefault(schema =>
            string.Equals(schema.Key, key, StringComparison.OrdinalIgnoreCase));

        return definition ?? throw new KeyNotFoundException($"Schema '{key}' was not found.");
    }
}
