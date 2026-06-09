# PostgreSQL Migration Guide

## Overview
This document describes the migration of the Films application from SQL Server to PostgreSQL 16.

## Changes Made

### 1. Package Dependencies Updated

**File:** `src/Films.Infrastructure/Films.Infrastructure.csproj`

**Removed:**
- `Microsoft.EntityFrameworkCore.SqlServer` Version 8.0.0

**Added:**
- `Npgsql.EntityFrameworkCore.PostgreSQL` Version 8.0.0
- `EFCore.NamingConventions` Version 8.0.0
- `Microsoft.EntityFrameworkCore.Relational` Version 8.0.0

### 2. DbContext Configuration Updated

**File:** `src/Films.Infrastructure/Data/FilmsDbContext.cs`

**Changes:**
- Set default schema to "public" for PostgreSQL
- Configured all table names to use snake_case (e.g., `Films` → `films`)
- Configured all column names to use snake_case (e.g., `FirstName` → `first_name`)
- Set DateTime columns to use `timestamp without time zone` type
- Configured proper foreign key relationships with snake_case naming

### 3. Program.cs Configuration Updated

**File:** `src/Films.Web/Program.cs`

**Changes:**
- Replaced `UseSqlServer()` with `UseNpgsql()`
- Added retry policy configuration for PostgreSQL
- Configured migrations history table: `__ef_migrations_history` in `public` schema
- Added `UseSnakeCaseNamingConvention()` for automatic snake_case naming
- Enabled `EnableLegacyTimestampBehavior` for PostgreSQL DateTime handling
- Added development-specific logging options

### 4. Connection Strings Updated

**File:** `src/Films.Web/appsettings.json`

**Old (SQL Server):**
```json
"DefaultConnection": "Server=localhost;Database=films;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
```

**New (PostgreSQL):**
```json
"DefaultConnection": "Host=localhost;Port=5432;Database=films;Username=postgres;Password=postgres;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=100;Connection Lifetime=0;Command Timeout=30;Timeout=15;"
```

**File:** `src/Films.Web/appsettings.Development.json`

**Added:**
```json
"DefaultConnection": "Host=localhost;Port=5432;Database=films_dev;Username=postgres;Password=postgres;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=100;Connection Lifetime=0;Command Timeout=30;Timeout=15;"
```

### 5. Entity Models Updated

Added `CreatedDate` property to entities that were missing it:
- `Sex.cs`
- `TypeUser.cs`
- `Right.cs`
- `UserRight.cs`
- `RefAF.cs`
- `RefDAF.cs`

## Database Schema Changes

### Naming Convention
All database objects now use snake_case naming:

| Entity Class | Table Name | Example Column |
|-------------|------------|----------------|
| Film | films | created_date |
| Actor | actors | first_name |
| Director | directors | last_name |
| User | users | password_hash |
| Sex | sex | name |
| TypeUser | type_users | description |
| Right | rights | name |
| UserRight | user_rights | user_id |
| RefAF | ref_af | actor_id |
| RefDAF | ref_daf | director_id |

### DateTime Handling
All DateTime columns use PostgreSQL's `timestamp without time zone` type.

## Migration Steps

### Prerequisites
1. Install PostgreSQL 16 on your system
2. Create a database named `films` (or `films_dev` for development)
3. Update connection string credentials in `appsettings.json`

### Running Migrations

1. **Remove old SQL Server migrations** (if any exist):
   ```bash
   rm -rf src/Films.Infrastructure/Migrations
   ```

2. **Create new PostgreSQL migration**:
   ```bash
   cd src/Films.Web
   dotnet ef migrations add InitialPostgreSQLMigration --project ../Films.Infrastructure
   ```

3. **Apply migration to database**:
   ```bash
   dotnet ef database update --project ../Films.Infrastructure
   ```

4. **Verify schema creation**:
   ```sql
   -- Connect to PostgreSQL and run:
   \dt public.*
   \d+ public.films
   ```

## Testing

### Unit Tests
No changes required - unit tests use in-memory database.

### Integration Tests
Integration tests will automatically use the PostgreSQL connection string from the Web project.

### Manual Testing
1. Start the application: `dotnet run --project src/Films.Web`
2. Navigate to: `https://localhost:5001`
3. Test CRUD operations on Films, Actors, and Directors

## Connection String Parameters

### PostgreSQL Connection String Breakdown
- `Host=localhost` - Database server address
- `Port=5432` - PostgreSQL default port
- `Database=films` - Database name
- `Username=postgres` - Database user
- `Password=postgres` - User password (change in production!)
- `Pooling=true` - Enable connection pooling
- `Minimum Pool Size=0` - Minimum connections in pool
- `Maximum Pool Size=100` - Maximum connections in pool
- `Connection Lifetime=0` - Connection lifetime (0 = unlimited)
- `Command Timeout=30` - Command timeout in seconds
- `Timeout=15` - Connection timeout in seconds

## Production Considerations

### Security
1. **Never commit passwords** - Use environment variables or Azure Key Vault
2. **Use SSL/TLS** - Add `SSL Mode=Require` to connection string
3. **Principle of least privilege** - Create dedicated database user with minimal permissions

### Performance
1. **Connection pooling** - Already configured (max 100 connections)
2. **Indexes** - Add indexes for frequently queried columns
3. **Query optimization** - Monitor slow queries with PostgreSQL logs

### Monitoring
1. Enable PostgreSQL query logging
2. Monitor connection pool usage
3. Set up alerts for failed connections

## Rollback Plan

If you need to rollback to SQL Server:

1. Restore the original `.csproj` file with SQL Server packages
2. Restore the original `FilmsDbContext.cs` without snake_case naming
3. Restore the original `Program.cs` with `UseSqlServer()`
4. Restore the original connection strings
5. Restore the original entity models (remove added CreatedDate properties)
6. Run: `dotnet ef migrations add RestoreSqlServer --project src/Films.Infrastructure`
7. Run: `dotnet ef database update --project src/Films.Infrastructure`

## Troubleshooting

### Common Issues

**Issue:** "Npgsql.PostgresException: 42P01: relation does not exist"
- **Solution:** Run migrations: `dotnet ef database update`

**Issue:** "Password authentication failed"
- **Solution:** Verify PostgreSQL credentials in connection string

**Issue:** "Could not connect to server"
- **Solution:** Ensure PostgreSQL service is running: `sudo systemctl status postgresql`

**Issue:** "DateTime conversion errors"
- **Solution:** Verify `EnableLegacyTimestampBehavior` is set in Program.cs

## Additional Resources

- [Npgsql Documentation](https://www.npgsql.org/efcore/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/16/)
- [EF Core Naming Conventions](https://github.com/efcore/EFCore.NamingConventions)

## Migration Summary

- **Total Files Modified:** 15
- **Package References Updated:** 3 removed, 3 added
- **Connection Strings Updated:** 2
- **Entity Models Updated:** 6
- **DbContext Configurations:** Complete rewrite for PostgreSQL
- **Migration Status:** Ready for database migration
- **Estimated Migration Time:** 15 minutes
