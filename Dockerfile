# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["BookReviewApp/BookReviewApp.csproj", "BookReviewApp/"]
COPY ["BookReviewApp.Tests/BookReviewApp.Tests.csproj", "BookReviewApp.Tests/"]
RUN dotnet restore "BookReviewApp/BookReviewApp.csproj"

# Copy source code and build
COPY . .
WORKDIR "/src/BookReviewApp"
RUN dotnet build "BookReviewApp.csproj" -c Release -o /app/build

# Test stage
FROM build AS test
WORKDIR /src
RUN dotnet test "BookReviewApp.Tests/BookReviewApp.Tests.csproj" --logger trx --results-directory /testresults

# Publish stage
FROM build AS publish
RUN dotnet publish "BookReviewApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser
RUN mkdir -p /app/logs && chown -R appuser:appuser /app

# Copy published app
COPY --from=publish /app/publish .

# Switch to non-root user
USER appuser

# Configure environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "BookReviewApp.dll"]