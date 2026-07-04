# SchemaForge

SchemaForge is an enterprise JSON Schema designer and document validation platform built with ASP.NET Core.

The long-term goal is to help teams ingest real business documents, extract structured data, design and version schemas, validate payloads, and export trusted results through modern APIs and reports.

## Vision

SchemaForge is being designed like a production-grade backend product, not a throwaway demo.

Planned capabilities:

- PDF upload and document intake
- PDF text extraction
- JSON Schema design and schema library management
- JSON document validation
- JWT authentication and authorization
- REST API with Swagger/OpenAPI
- Export to JSON, CSV, and Excel
- Schema versioning and lifecycle management
- Validation reports and auditability

## Current Status

The repository currently includes:

- `SchemaForge.API`: ASP.NET Core Web API
- Lesson 1 employee sample JSON and manual JSON Schema
- Lesson endpoints for retrieving the sample payload and schema
- Swagger UI setup
- Initial automated tests and CI scaffolding

## Solution Structure

- `SchemaForge.API/`: application API
- `SchemaForge.API/Schemas/`: JSON Schema files used by lessons and platform features
- `SchemaForge.API/Samples/`: example JSON payloads
- `SchemaForge.Tests/`: automated tests
- `docs/`: product and engineering documentation

## Quick Start

### Prerequisites

- .NET 10 SDK
- Git

Optional local tools:

- Node.js LTS
- Postman or Thunder Client

### Run The API

```bash
dotnet run --project SchemaForge.API
```

Open Swagger UI:

```text
https://localhost:xxxx/swagger
```

### Run Tests

```bash
dotnet test SchemaForge.slnx
```

## Current Lesson Endpoints

- `GET /api/lessons/employee/sample`
- `GET /api/lessons/employee/schema`
- `GET /api/lessons/employee/schema/summary`

## Roadmap

Near-term milestones:

1. Add a real JSON validation endpoint using `JsonSchema.Net`
2. Introduce application services and domain models for schema management
3. Add persistence with SQL Server
4. Add JWT authentication and protected schema workflows
5. Add document upload, extraction, and export pipelines

See [Product Roadmap](docs/roadmap.md) and [Architecture Notes](docs/architecture.md) for more detail.

## Open Source

SchemaForge is being developed as an open-source portfolio-quality backend project with documentation, tests, and CI from the start.

Contributions, issues, and suggestions are welcome. See [Contributing Guide](docs/CONTRIBUTING.md).

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE).
