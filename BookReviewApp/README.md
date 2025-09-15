# Book Review App

A comprehensive book review application built with ASP.NET Core 9, featuring both MVC and REST API interfaces.

## Features

### Core Functionality
- **User Authentication**: ASP.NET Core Identity integration
- **Book Management**: CRUD operations for books
- **Review System**: Users can write and rate books (1-5 stars)
- **Voting System**: Like/dislike reviews (one vote per user per review)
- **Search & Filtering**: Filter books by genre, year, rating
- **Responsive UI**: Bootstrap 5 with modern design

### Technical Features
- **Clean Architecture**: Separation of concerns with Services layer
- **Entity Framework Core**: Code-First with migrations
- **Dual Interface**: Both MVC views and REST API
- **Input Validation**: Client and server-side validation
- **Error Handling**: Comprehensive error handling and logging
- **Unit Testing**: Full test coverage with xUnit

## Technology Stack

- **Backend**: ASP.NET Core 9 MVC
- **Database**: SQL Server with Entity Framework Core
- **Frontend**: Razor Views with Bootstrap 5
- **API**: RESTful Web API with Swagger documentation
- **Testing**: xUnit, FluentAssertions, Moq
- **Authentication**: ASP.NET Core Identity

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- SQL Server (LocalDB or full version)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
```bash
git clone [repository-url]
cd BookReviewApp