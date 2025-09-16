## CI/CD Status

[![CI/CD](https://github.com/Antonios-Georgiopoulos/BookReviewApp/actions/workflows/ci-cd.yml/badge.svg?branch=main)](https://github.com/Antonios-Georgiopoulos/BookReviewApp/actions/workflows/ci-cd.yml)
![Last Commit](https://img.shields.io/github/last-commit/Antonios-Georgiopoulos/BookReviewApp)

## User Documentation

- 📖 [User Guide](USER_GUIDE.md)

# BookReviewApp

A modern, full-stack book review application built with ASP.NET Core 9, featuring a comprehensive REST API, web interface, and production-ready architecture.

## Architecture & Features

### Core Technologies

- **Backend**: ASP.NET Core 9 MVC + REST API
- **Database**: Entity Framework Core with SQL Server
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Razor Views with Bootstrap 5 + JavaScript
- **Caching**: In-Memory caching with invalidation strategies
- **Logging**: Serilog with file rotation
- **Testing**: xUnit with FluentAssertions and Moq
- **Containerization**: Docker with multi-stage builds
- **CI/CD**: GitHub Actions with automated testing and deployment

### Advanced Architecture Patterns

- **Repository Pattern** with Unit of Work
- **CQRS-inspired** service layer separation
- **Dependency Injection** with custom service extensions
- **Decorator Pattern** for caching services
- **Clean Architecture** principles
- **AutoMapper** for object-object mapping

### Production-Ready Features

- **Health Checks** with custom database and cache monitoring
- **Rate Limiting** (100 requests/minute for API endpoints)
- **API Versioning** (URL, query string, and header support)
- **Swagger Documentation** with security definitions
- **Structured Logging** with correlation IDs
- **Docker Containerization** with security best practices
- **Automated CI/CD** pipeline with security scanning

## Quick Start

### Prerequisites

- .NET 9 SDK
- SQL Server (LocalDB for development)
- Docker (optional, for containerized deployment)

### Local Development

```bash
# Clone the repository
git clone https://github.com/yourusername/BookReviewApp.git
cd BookReviewApp

# Restore dependencies
dotnet restore

# Update database
dotnet ef database update --project BookReviewApp --context ApplicationDbContext

# Run the application
dotnet run --project BookReviewApp
```

The application will be available at:

- Web Interface: https://localhost:7297
- API Documentation: https://localhost:7297/swagger
- Health Checks UI: https://localhost:7297/health-ui

### Docker Deployment

```bash
# Build and run with Docker Compose
docker-compose up --build

# Access the application
# Web Interface: http://localhost:8080
# Health Checks: http://localhost:8080/health
```

## API Documentation

### Authentication Endpoints

The application uses ASP.NET Core Identity for authentication. Access `/Identity/Account/Register` and `/Identity/Account/Login` for user management.

### Book Management API

- `GET /api/v1/books` - List books with filtering and pagination
- `GET /api/v1/books/{id}` - Get book details
- `POST /api/v1/books` - Create new book (requires authentication)
- `PUT /api/v1/books/{id}` - Update book (requires authentication)
- `DELETE /api/v1/books/{id}` - Delete book (requires authentication)

### Review Management API

- `GET /api/v1/books/{bookId}/reviews` - Get reviews for a book
- `GET /api/v1/reviews/{id}` - Get specific review
- `POST /api/v1/reviews` - Create review (requires authentication)
- `PUT /api/v1/reviews/{id}` - Update review (requires authentication)
- `DELETE /api/v1/reviews/{id}` - Delete review (requires authentication)
- `POST /api/v1/reviews/{id}/vote` - Vote on review (requires authentication)

### Response Format

All API responses follow a consistent format:

```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": {
    /* response data */
  },
  "errors": [],
  "timestamp": "2025-01-15T10:30:00Z",
  "traceId": "correlation-id"
}
```

## Architecture Deep Dive

### Service Layer Architecture

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Controllers   │───▶│   Service Layer  │───▶│  Repository     │
│   (API/MVC)     │    │   (Business)     │    │  (Data Access)  │
└─────────────────┘    └──────────────────┘    └─────────────────┘
         │                       │                        │
         ▼                       ▼                        ▼
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   ViewModels/   │    │   Caching Layer  │    │   Entity        │
│   DTOs          │    │   (Decorator)    │    │   Framework     │
└─────────────────┘    └──────────────────┘    └─────────────────┘
```

### Database Schema

```sql
Users (ASP.NET Identity)
├── Books
│   ├── Reviews
│   │   └── ReviewVotes
│   └── (Average ratings calculated)
```

### Caching Strategy

- **Book listings**: 15 minutes TTL with genre/year/rating invalidation
- **Individual books**: 30 minutes TTL with update invalidation
- **Metadata**: 2 hours TTL (genres, years)
- **User-specific data**: No caching (privacy)

## Development Guidelines

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "BookServiceTests"
```

### Database Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName --project BookReviewApp --context ApplicationDbContext

# Update database
dotnet ef database update --project BookReviewApp --context ApplicationDbContext

# Remove last migration
dotnet ef migrations remove --project BookReviewApp --context ApplicationDbContext
```

### Code Quality

- Follow C# coding conventions and use `dotnet format`
- Maintain test coverage above 80%
- Use meaningful commit messages following Conventional Commits
- All API endpoints must have proper error handling and logging
- New features require corresponding unit tests

## Performance Considerations

### Implemented Optimizations

- **Database**: Proper indexing on Genre, PublishedYear, UserId, BookId
- **Caching**: Multi-layer caching with intelligent invalidation
- **Queries**: Optimized EF queries with selective includes
- **API**: Pagination for large datasets
- **Rate Limiting**: Prevents API abuse

### Monitoring & Observability

- **Health Checks**: `/health` endpoint with detailed status
- **Structured Logging**: Correlation IDs for request tracking
- **Performance Metrics**: Request duration logging
- **Error Tracking**: Comprehensive exception logging with context

## Security Features

### Authentication & Authorization

- ASP.NET Core Identity with customizable password policies
- JWT-ready architecture (can be extended)
- Role-based authorization support
- Anti-forgery tokens for web forms

### API Security

- Rate limiting to prevent abuse
- Input validation and sanitization
- CORS policies for cross-origin requests
- Security headers (can be extended with middleware)

### Container Security

- Non-root user execution in containers
- Multi-stage builds to minimize attack surface
- Security scanning in CI/CD pipeline
- Secrets management through environment variables

## Deployment

### Environment Configuration

- **Development**: LocalDB with detailed logging
- **Production**: SQL Server with optimized logging and caching
- **Docker**: Containerized with external SQL Server

### CI/CD Pipeline Features

- Automated testing on all branches
- Security vulnerability scanning
- Docker image building and registry push
- Staged deployments with health checks
- Automated rollback capabilities

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'feat: add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License.
