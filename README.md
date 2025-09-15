# Book Review App - Assessment Project

Comprehensive book review application built with ASP.NET Core 9 MVC + REST API, developed as an assessment project covering all modern web development requirements.

## Architecture & Technical Choices

### Clean Architecture Implementation
- **Controllers**: Separation of MVC and API controllers for clean separation of concerns
- **Services Layer**: Business logic decoupled from presentation layer
- **Repository Pattern**: Through Entity Framework Core for data abstraction
- **Dependency Injection**: Native ASP.NET Core DI container

### Authentication & Authorization
```csharp
// ASP.NET Core Identity chosen for production-ready security
builder.Services.AddIdentity<User, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    // Balanced security vs usability
})
```

### Database Design & Migrations
- **Code-First Approach**: Entity configurations in `ApplicationDbContext`
- **Indexes**: Strategically placed for performance (Genre, PublishedYear, DateCreated)
- **Cascade Deletes**: Proper data integrity with DeleteBehavior.Cascade/NoAction
- **Seeded Data**: Production-like test data without hardcoded users

## Core Functionality

### Core Features (100% Assessment Requirements)
- ✅ **User Registration/Login**: ASP.NET Identity integration
- ✅ **Books CRUD**: Full MVC interface + REST API
- ✅ **Reviews System**: 1-5 star ratings with content validation
- ✅ **Voting System**: One vote per user per review with business rules
- ✅ **Filtering**: Genre, year, rating with efficient querying

### API Design
```
GET    /api/books                 # List with filtering
GET    /api/books/{id}            # Details with reviews
POST   /api/books                 # Create (Auth required)
GET    /api/books/{id}/reviews    # Reviews for book
POST   /api/reviews               # Create review (Auth required)  
POST   /api/reviews/{id}/vote     # Vote (Auth required)
```

### Frontend Approach
- **Razor Views**: Server-side rendering for SEO and performance
- **Bootstrap 5**: Responsive design without custom CSS framework overhead
- **Progressive Enhancement**: JavaScript for UX improvements, not dependencies
- **Validation**: Unobtrusive client-side + server-side validation

## Technical Requirements - Implementation

### ASP.NET Core 9 + EF Core
```csharp
// Modern async/await patterns across all layers
public async Task<BookDto?> GetBookByIdAsync(int id)
{
    var book = await _context.Books
        .Include(b => b.Reviews)
        .FirstOrDefaultAsync(b => b.Id == id);
    
    return book == null ? null : MapToDto(book);
}
```

### Error Handling & Logging
- **BaseController**: Centralized error handling for API
- **Structured Logging**: ILogger dependency injection
- **Exception Filters**: Global exception handling
- **Validation**: ModelState validation with descriptive error messages

### Testing Strategy
- **Unit Tests**: Services layer with Moq for dependencies
- **Integration Tests**: In-memory database for controllers
- **Test Coverage**: Core business logic and edge cases

## Installation & Setup

```bash
# Clone repository
git clone [repository-url]
cd BookReviewApp

# Restore packages
dotnet restore

# Update connection string in appsettings.json
# Apply migrations
dotnet ef database update

# Run application
dotnet run
```

### Development Tools
- **Swagger UI**: `/swagger` endpoint for API documentation
- **EF Migrations**: `dotnet ef migrations add` for schema changes
- **LocalDB**: Development database without external dependencies

## Project Structure

```
BookReviewApp/
├── Controllers/
│   ├── Api/              # RESTful API controllers
│   └── MVC/              # Traditional MVC controllers
├── Services/             # Business logic layer
├── Data/                 # EF Context, migrations, seeding
├── Models/
│   ├── Domain/           # Entity models
│   └── ViewModels/       # DTOs and view models
└── Views/                # Razor templates
```

## Advanced Features

### Performance Considerations
- **Eager Loading**: Strategic `Include()` for N+1 prevention
- **Pagination**: Server-side paging for large datasets  
- **Caching Strategy**: Ready for output caching implementation

### Security Implementation
- **HTTPS Enforced**: Redirect middleware
- **AntiForgery Tokens**: CSRF protection
- **Authorization Policies**: Method-level security
- **Input Validation**: Comprehensive model validation

### Code Quality
- **SOLID Principles**: Dependency inversion, single responsibility
- **Async Best Practices**: ConfigureAwait(false) where necessary
- **Error Boundaries**: Graceful degradation
- **Separation of Concerns**: Clear layer boundaries

## API Documentation

Swagger UI available at `/swagger` with complete documentation of all endpoints, request/response schemas and authentication requirements.

## Testing

```bash
# Run all tests
dotnet test

# Test coverage includes:
# - Service layer business logic
# - API controller responses  
# - Data layer operations
# - Authentication/authorization flows
```