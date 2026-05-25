# Multi-stage Dockerfile for FIlms ASP.NET Web Application
# Stage 1: Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

# Set working directory
WORKDIR /src

# Copy project file first for dependency caching
COPY FIlms.csproj ./
COPY FIlms.sln ./

# Restore NuGet packages
RUN dotnet restore FIlms.csproj

# Copy the rest of the application source code
COPY . .

# Build the application in Release mode
RUN dotnet build FIlms.csproj -c Release -o /app/build

# Publish the application
RUN dotnet publish FIlms.csproj -c Release -o /app/publish --no-restore

# Stage 2: Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set working directory
WORKDIR /app

# Create non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Create necessary directories with proper permissions
RUN mkdir -p /app/data/files /app/logs /app/temp && \
    chown -R appuser:appuser /app

# Copy published application from builder stage
COPY --from=builder /app/publish .

# Set ownership to non-root user
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Set environment variables for ASP.NET Core
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Expose application port
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "FIlms.dll"]
