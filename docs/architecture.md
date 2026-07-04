# Architecture Notes

## Product Direction

SchemaForge is intended to evolve into a modular backend platform for schema-first document intelligence.

The platform will center around these bounded areas:

- Document ingestion
- Extraction and normalization
- Schema design and versioning
- Validation and reporting
- Export and integration
- Identity and access control

## Proposed Backend Building Blocks

## API Layer

- Minimal APIs or controllers for public HTTP contracts
- Swagger/OpenAPI as the living API catalog
- JWT bearer authentication for protected endpoints

## Application Layer

- Use-case services for schema creation, validation, export, and reporting
- DTOs for request and response contracts
- Workflow orchestration for document processing pipelines

## Domain Layer

- Schema definitions
- Schema versions
- Validation jobs
- Validation reports
- Export artifacts

## Infrastructure Layer

- SQL Server persistence
- File storage for uploaded documents
- Background jobs for extraction and exports
- External AI integrations for structured extraction

## Suggested Early Modules

1. Employee schema validation
2. Hospital patient schema validation
3. Payroll schema validation
4. Invoice schema validation
5. Bank statement schema validation

## Engineering Standards

- Prefer explicit contracts and typed responses
- Keep schema assets versioned in source control
- Add tests for validation rules and edge cases
- Maintain CI green on every push
- Document architectural decisions as the platform grows
