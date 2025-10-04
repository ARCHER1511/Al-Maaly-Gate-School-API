📘 Al-Maaly-Gate-School Solution Guide

This solution follows Onion Architecture in ASP.NET Core, designed to separate concerns and enforce dependency flow (outer layers → inner layers).

🏛 Project Structure
1. Domain (Core Business Layer)

Purpose: Contains the business logic of the application. No external dependencies (keep it pure C#).

References: No dependencies. Other projects depend on this.

Suggested folders:

Entities/ → Domain entities (Student, Teacher, Class, etc.)

ValueObjects/ → Immutable business concepts (Email, PhoneNumber, Grade)

Enums/ → Domain enums (Gender, UserRole)

Interfaces/ → Abstractions for repositories or domain services (IStudentRepository)

Packages: None (keep it lightweight).

2. Application (Use Cases Layer)

Purpose: Coordinates business rules into use cases / services. Defines contracts (DTOs, interfaces).

References: Depends only on Domain.

Suggested folders:

DTOs/ → Data transfer objects (StudentDto, TeacherDto)

Services/ → Application services (StudentService, EnrollmentService)

Interfaces/ → Contracts for application services (IStudentService)

Mappings/ → Manual mappers or helper classes for entity-to-DTO conversion

Packages:

FluentValidation (for input validation)

MediatR (optional, for CQRS / request-response pattern)

3. Infrastructure (Data & External Layer)

Purpose: Implements domain/application interfaces using EF Core, persistence, external services.

References: Depends on Application and Domain.

Suggested folders:

Data/ → AppDbContext, Migrations

Repositories/ → EF Core implementations of IStudentRepository, etc.

Services/ → Infrastructure services (Email sender, File storage, etc.)

Configurations/ → EF Core entity configurations (StudentConfig)

Packages:

Microsoft.EntityFrameworkCore

Microsoft.EntityFrameworkCore.SqlServer 

Microsoft.EntityFrameworkCore.Tools

Microsoft.Extensions.Logging

4. Common (Cross-Cutting Concerns)

Purpose: Shared utilities and cross-cutting features used by all layers.

References: Referenced by Application, Infrastructure, and Web.

Suggested folders:

Wrappers/ → Standard API responses (GeneralResponse, Result<T>)

Exceptions/ → Custom exception types (NotFoundException, ValidationException)

Extensions/ → Extension methods (StringExtensions, DateTimeExtensions)

Constants/ → Shared constants (Roles, AppSettingsKeys)

Interfaces/ → Shared cross-cutting contracts (IDateTimeProvider, ILoggerAdapter)

Packages:

None required, unless utility-specific.

5. Web/API (to be added)

(not in screenshot, but usually you’ll add it)

Purpose: ASP.NET Core Web API / MVC project. Entry point of the application.

References: Depends on Application (and indirectly uses Infrastructure via DI).

Suggested folders:

Controllers/ → API controllers (StudentsController, TeachersController)

Middlewares/ → Error handling, logging, JWT middleware

Filters/ → Validation filters, authorization filters

Packages:

Microsoft.AspNetCore.Authentication.JwtBearer (for JWT auth)

Swashbuckle.AspNetCore (Swagger/OpenAPI)

Serilog.AspNetCore (for logging)

🔄 Dependency Flow
Web/API → Application → Domain
       ↘ Infrastructure ↗


Domain is pure and doesn’t depend on anyone.

Application depends only on Domain.

Infrastructure implements Application/Domain contracts.

Web/API wires everything up using Dependency Injection.

🚀 How to Run

Set up Infrastructure → configure AppDbContext with your database.

Add Web/API project (if not already).

In Program.cs (Web), register services:

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();


Run migrations:

dotnet ef migrations add InitialCreate -p Infrastructure -s Web
dotnet ef database update -p Infrastructure -s Web


Start the API → check Swagger UI.

✅ Best Practices

Keep Domain clean (no EF annotations, no ASP.NET).

Use Fluent API in Infrastructure for entity configurations.

Return DTOs from Application, not entities.

Use Common only for truly shared things (avoid dumping everything).

Test Domain and Application with unit tests, Infrastructure with integration tests.

👉 This README serves as a developer guide so anyone joining the project understands where to put new files and how dependencies should flow.