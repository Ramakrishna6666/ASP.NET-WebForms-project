# Films Management System

## Overview
This is a modern .NET 8 web application for managing films, actors, and directors. The application has been migrated from ASP.NET Web Forms 4.6 to .NET 8 using clean architecture principles.

## Architecture
The solution follows clean architecture with the following layers:

- **Films.Domain**: Core domain entities and interfaces
- **Films.Application**: Business logic, services, and DTOs
- **Films.Infrastructure**: Data access, repositories, and EF Core
- **Films.Web**: Razor Pages UI layer

## Technologies
- .NET 8
- ASP.NET Core Razor Pages
- Entity Framework Core 8.0
- SQL Server
- AutoMapper
- Serilog
- xUnit, FluentAssertions, Moq

## Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

## Getting Started

### 1. Update Connection String
Edit `src/Films.Web/appsettings.json` and update the connection string:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=films;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

### 2. Create Database
Run the following commands from the solution root:
```bash
cd src/Films.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../Films.Web
dotnet ef database update --startup-project ../Films.Web
```

### 3. Run the Application
```bash
cd src/Films.Web
dotnet run
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`.

## Project Structure
```
Films/
├── src/
│   ├── Films.Domain/          # Domain entities and interfaces
│   ├── Films.Application/     # Business logic and services
│   ├── Films.Infrastructure/  # Data access and repositories
│   └── Films.Web/             # Razor Pages UI
├── tests/
│   ├── Films.UnitTests/       # Unit tests
│   └── Films.IntegrationTests/# Integration tests
└── Films.sln                  # Solution file
```

## Features
- Film management (CRUD operations)
- Actor management
- Director management
- Clean architecture
- Dependency injection
- Logging with Serilog
- Entity Framework Core
- Async/await patterns
- Unit and integration tests

## Migration Notes
This application was migrated from ASP.NET Web Forms 4.6 to .NET 8. Key changes include:

- Replaced Web Forms pages with Razor Pages
- Migrated from Entity Framework 6 to EF Core 8
- Replaced Web.config with appsettings.json
- Implemented clean architecture
- Added dependency injection
- Replaced ViewState with modern state management
- Updated authentication to ASP.NET Core Identity (if needed)

## Testing
Run unit tests:
```bash
dotnet test tests/Films.UnitTests
```

Run integration tests:
```bash
dotnet test tests/Films.IntegrationTests
```

Run all tests:
```bash
dotnet test
```

## Build
Build the solution:
```bash
dotnet build
```

Build in Release mode:
```bash
dotnet build --configuration Release
```

## License
This project is for educational purposes.
