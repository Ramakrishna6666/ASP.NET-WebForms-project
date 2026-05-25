# Containerization Changes

## Overview
This document describes the changes made to prepare the ASP.NET WebForms application for containerization in AWS ECS/EKS.

## Changes Made

### 1. Drive Letter Dependencies Fix (Blocker: cz-dotnet-0006)

**Problem**: Hardcoded Windows drive letter paths (C:\, D:\) cause file system access failures in Linux containers.

**Solution**: 
- Created `ConfigurationHelper.cs` - A utility class that reads configuration from environment variables or Web.config
- Updated `Default.aspx.cs` to demonstrate proper usage of externalized file paths
- Added `appSettings` section in `Web.config` with Linux-compatible default paths

**Files Modified**:
- `ConfigurationHelper.cs` (NEW) - Configuration helper for environment variable support
- `Default.aspx.cs` - Updated to use ConfigurationHelper for file paths
- `Web.config` - Added appSettings section with file path configurations

**Environment Variables**:
The following environment variables can be set in the container to override default paths:
- `DataFilePath` - Path for data files (default: /app/data/files)
- `LogFilePath` - Path for log files (default: /app/logs)
- `TempFilePath` - Path for temporary files (default: /app/temp)

**Usage Example**:
```csharp
// Instead of hardcoded path:
// string dataPath = "C:\\Data\\Files";

// Use ConfigurationHelper:
string dataPath = ConfigurationHelper.GetFilePath("DataFilePath", "/app/data/files");
```

### 2. Health Check Endpoint (Mandatory for Containerization)

**Purpose**: Container orchestration platforms (ECS/EKS) require health check endpoints to monitor application health.

**Implementation**:
- Created `HealthCheck.aspx` - Health check endpoint page
- Created `HealthCheck.aspx.cs` - Health check logic returning JSON status
- Created `HealthCheck.aspx.designer.cs` - Designer file

**Endpoint**: `/HealthCheck.aspx`

**Response Format**:
```json
{
  "status": "healthy",
  "timestamp": "2024-01-15T10:30:00.000Z"
}
```

**HTTP Status Codes**:
- 200 OK - Application is healthy
- 503 Service Unavailable - Application is unhealthy

**Container Configuration**:
Add this to your ECS task definition or Kubernetes deployment:
```yaml
healthCheck:
  command: ["CMD-SHELL", "curl -f http://localhost/HealthCheck.aspx || exit 1"]
  interval: 30s
  timeout: 5s
  retries: 3
  startPeriod: 60s
```

### 3. Project File Updates

**File**: `FIlms.csproj`

**Changes**:
- Added `ConfigurationHelper.cs` to compilation
- Added `HealthCheck.aspx`, `HealthCheck.aspx.cs`, and `HealthCheck.aspx.designer.cs` to project
- Maintained all existing references and dependencies

## Deployment Considerations

### Environment Variables for Container
Set these environment variables in your container runtime:

```bash
# File paths (Linux-compatible)
DataFilePath=/app/data/files
LogFilePath=/app/logs
TempFilePath=/app/temp

# Connection strings (override Web.config)
ConnectionStrings__DefaultConnection="Server=${DB_HOST};Database=${DB_NAME};User Id=${DB_USER};Password=${DB_PASSWORD};"
ConnectionStrings__filmsConnectionString="Server=${DB_HOST};Database=films;User Id=${DB_USER};Password=${DB_PASSWORD};"
```

### Docker Volume Mounts
Ensure these directories are mounted or created in the container:
```dockerfile
VOLUME ["/app/data", "/app/logs", "/app/temp"]
```

### ECS/EKS Configuration
1. **Health Check**: Configure the health check to use `/HealthCheck.aspx`
2. **Environment Variables**: Set all required environment variables in task definition
3. **Volumes**: Mount EFS or persistent volumes for data directories
4. **Secrets**: Use AWS Secrets Manager for sensitive configuration (connection strings, API keys)

## Testing

### Local Testing
1. Set environment variables before running:
   ```powershell
   $env:DataFilePath = "C:\temp\data"
   $env:LogFilePath = "C:\temp\logs"
   ```

2. Test health check endpoint:
   ```bash
   curl http://localhost/HealthCheck.aspx
   ```

### Container Testing
1. Build Docker image
2. Run container with environment variables:
   ```bash
   docker run -e DataFilePath=/app/data -e LogFilePath=/app/logs -p 80:80 your-image
   ```

3. Verify health check:
   ```bash
   curl http://localhost/HealthCheck.aspx
   ```

## Migration Notes

### Before Containerization
- Application used hardcoded Windows paths
- No health check endpoint
- Configuration was static in Web.config

### After Containerization
- All file paths are externalized and configurable via environment variables
- Health check endpoint available at `/HealthCheck.aspx`
- Configuration supports both Web.config and environment variables
- Linux-compatible path handling

## Blocker Resolution Summary

| Blocker ID | Rule ID | Description | Status | Resolution |
|------------|---------|-------------|--------|------------|
| blocker-1 | cz-dotnet-0006 | Drive Letter Dependencies | ✅ Fixed | Created ConfigurationHelper for externalized file paths |
| N/A | health-check | Health Check Endpoint | ✅ Added | Created /HealthCheck.aspx endpoint |

## Next Steps

1. **Database Configuration**: Update connection strings to use environment variables for database host, credentials
2. **Secrets Management**: Integrate with AWS Secrets Manager for sensitive configuration
3. **Logging**: Configure logging to write to stdout/stderr for container log aggregation
4. **Monitoring**: Add application metrics and integrate with CloudWatch or Prometheus
5. **Load Testing**: Perform load testing in containerized environment
