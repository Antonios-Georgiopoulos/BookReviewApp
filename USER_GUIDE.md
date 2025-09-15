# BookReviewApp User Guide

A comprehensive guide to using the Book Review Application - both the web interface and REST API.

## Table of Contents

1. [Getting Started](#getting-started)
2. [Web Interface Guide](#web-interface-guide)
3. [REST API Guide](#rest-api-guide)
4. [User Management](#user-management)
5. [Book Management](#book-management)
6. [Review System](#review-system)
7. [Advanced Features](#advanced-features)
8. [Troubleshooting](#troubleshooting)

## Getting Started

### Account Registration

1. **Navigate to Registration**

   - Visit the application homepage
   - Click "Register" in the top navigation
   - Or go directly to `/Identity/Account/Register`

2. **Create Your Account**

   - Enter a unique username
   - Provide a valid email address
   - Create a strong password (minimum 6 characters, must include uppercase, lowercase, and digits)
   - Click "Register"

3. **First Login**
   - Use your credentials at `/Identity/Account/Login`
   - You'll be redirected to the books listing page

### System Requirements

- Modern web browser (Chrome, Firefox, Safari, Edge)
- JavaScript enabled
- Internet connection for external resources (Bootstrap, FontAwesome)

## Web Interface Guide

### Homepage Overview

The homepage provides three main pathways:

- **Discover Books**: Browse the book catalog
- **Write Reviews**: Share your thoughts (requires login)
- **Join Community**: Registration prompt for new users

### Books Section

#### Browsing Books

1. **Main Books Page** (`/Books`)

   - View all books in a grid or list layout
   - Use the view toggle buttons (grid/list) in the top-right

2. **Filtering Options**

   - **Search**: Enter title or author in the search box
   - **Genre**: Select from dropdown (Fiction, Fantasy, Science Fiction, etc.)
   - **Year**: Filter by publication year
   - **Rating**: Set minimum rating threshold (1-5 stars)
   - **Sorting**: Choose sort criteria (Title, Author, Year, Rating, Date Added)
   - **Order**: Ascending or descending

3. **Pagination**
   - Navigate through results using page numbers
   - Default: 10 books per page

#### Book Details

1. **Viewing Details**

   - Click any book title or "View" button
   - See complete information: title, author, genre, publication year
   - View average rating and total review count
   - Read all user reviews

2. **Book Information Display**
   - Star ratings (visual and numerical)
   - Publication details
   - Date added to system
   - Complete review history

#### Adding Books (Authenticated Users)

1. **Access Creation Form**

   - Click "Add Book" in navigation or books page
   - Fill in required fields:
     - **Title**: Book title (max 200 characters)
     - **Author**: Author name (max 100 characters)
     - **Publication Year**: Between 1000-2100
     - **Genre**: Select from dropdown or enter custom

2. **Genre Selection**

   - Choose from predefined genres
   - Select "Other" to enter custom genre
   - Custom genre input appears dynamically

3. **Form Validation**
   - All fields with asterisk (\*) are required
   - Real-time validation feedback
   - Submission prevented if validation fails

#### Editing Books (Authenticated Users)

1. **Access Edit Form**

   - Go to book details page
   - Click "Edit" button (yellow warning button)

2. **Editing Process**
   - Modify any field except creation date and statistics
   - Warning displayed about affecting related reviews
   - Changes apply immediately after submission

#### Deleting Books (Authenticated Users)

1. **Deletion Process**
   - Click "Delete" button (red danger button) from book details
   - Review deletion consequences (affected reviews and votes)
   - Confirm deletion with double-confirmation prompt
   - **Warning**: This action is irreversible

### Review System

#### Reading Reviews

- All reviews visible on book details pages
- Reviews show:
  - User name and creation date
  - Star rating (1-5 stars)
  - Review content
  - Vote counts (upvotes/downvotes)
  - Net score

#### Writing Reviews (Authenticated Users)

1. **Review Creation**

   - Navigate to book details page
   - Use "Write your review" form (only if you haven't reviewed this book)
   - **Rating**: Click stars to select (1-5 stars)
   - **Content**: Write detailed review (10-2000 characters)

2. **Rating System**

   - Interactive star selection
   - Hover effects for user guidance
   - Visual feedback for selected rating

3. **Submission**
   - Click "Submit Review"
   - One review per user per book
   - Reviews appear immediately

#### Editing Your Reviews

1. **Access Edit Form**

   - Find your review on book details page
   - Click three-dots menu (⋮) next to your review
   - Select "Edit"

2. **Editing Process**
   - Modify rating and/or content
   - Character counter shows usage
   - Unsaved changes warning when navigating away

#### Deleting Your Reviews

1. **Deletion Process**
   - Use three-dots menu (⋮) next to your review
   - Click "Delete"
   - Confirm deletion in prompt
   - **Note**: Only your own reviews can be deleted

#### Voting on Reviews

1. **Vote Options** (for reviews by other users)

   - **Upvote**: Thumbs up (👍) - agree/helpful
   - **Downvote**: Thumbs down (👎) - disagree/not helpful

2. **Voting Rules**

   - Cannot vote on your own reviews
   - One vote per review per user
   - Can change vote (upvote ↔ downvote)
   - Click same vote to remove it

3. **Vote Display**
   - Green upvote count
   - Red downvote count
   - Net score (upvotes - downvotes)

## REST API Guide

### API Base Information

- **Base URL**: `https://yourdomain.com/api/v1/`
- **Authentication**: Bearer token (when required)
- **Content-Type**: `application/json`
- **Rate Limiting**: 100 requests per minute

### Authentication

#### API Authentication Flow

1. Register/Login through web interface
2. For API access, implement JWT token retrieval (future enhancement)
3. Currently uses cookie-based authentication from web login

### Books API Endpoints

#### GET /api/v1/books

List all books with optional filtering and pagination.

**Query Parameters:**

- `genre` (string): Filter by genre
- `year` (integer): Filter by publication year
- `minRating` (decimal): Minimum average rating
- `page` (integer): Page number (default: 1)
- `pageSize` (integer): Items per page (default: 10, max: 100)

**Example Request:**

```bash
curl -X GET "https://yourdomain.com/api/v1/books?genre=Fiction&minRating=4.0&page=1&pageSize=5"
```

**Response:**

```json
{
  "success": true,
  "message": "Data retrieved successfully",
  "data": [
    {
      "id": 1,
      "title": "Sample Book",
      "author": "John Doe",
      "publishedYear": 2023,
      "genre": "Fiction",
      "dateCreated": "2025-01-15T10:00:00Z",
      "averageRating": 4.5,
      "reviewCount": 10
    }
  ],
  "currentPage": 1,
  "pageSize": 5,
  "totalPages": 2,
  "totalCount": 8,
  "hasNextPage": true,
  "hasPreviousPage": false,
  "timestamp": "2025-01-15T12:00:00Z",
  "traceId": "correlation-id-123"
}
```

#### GET /api/v1/books/{id}

Get specific book details.

**Response:**

```json
{
  "success": true,
  "message": "Book retrieved successfully",
  "data": {
    "id": 1,
    "title": "Sample Book",
    "author": "John Doe",
    "publishedYear": 2023,
    "genre": "Fiction",
    "dateCreated": "2025-01-15T10:00:00Z",
    "averageRating": 4.5,
    "reviewCount": 10
  },
  "timestamp": "2025-01-15T12:00:00Z",
  "traceId": "correlation-id-123"
}
```

#### POST /api/v1/books

Create a new book (requires authentication).

**Request Body:**

```json
{
  "title": "New Book Title",
  "author": "Author Name",
  "publishedYear": 2024,
  "genre": "Fantasy"
}
```

**Response:** Returns created book with generated ID and creation timestamp.

#### PUT /api/v1/books/{id}

Update existing book (requires authentication).

**Request Body:** Same as POST
**Response:** Returns updated book information

#### DELETE /api/v1/books/{id}

Delete book (requires authentication).
**Response:** 204 No Content on success

### Reviews API Endpoints

#### GET /api/v1/books/{bookId}/reviews

Get all reviews for a specific book.

**Response:**

```json
{
  "success": true,
  "message": "Reviews retrieved successfully",
  "data": [
    {
      "id": 1,
      "content": "Great book! Highly recommended.",
      "rating": 5,
      "dateCreated": "2025-01-15T10:00:00Z",
      "bookId": 1,
      "bookTitle": "Sample Book",
      "userId": "user-id-123",
      "userName": "BookLover",
      "upvoteCount": 5,
      "downvoteCount": 1,
      "netVotes": 4
    }
  ]
}
```

#### POST /api/v1/reviews

Create new review (requires authentication).

**Request Body:**

```json
{
  "content": "This book was amazing! The character development was excellent.",
  "rating": 5,
  "bookId": 1
}
```

#### PUT /api/v1/reviews/{id}

Update your review (requires authentication).

#### DELETE /api/v1/reviews/{id}

Delete your review (requires authentication).

#### POST /api/v1/reviews/{id}/vote

Vote on a review (requires authentication).

**Request Body:**

```json
true // for upvote, false for downvote
```

### Error Handling

#### Standard Error Response

```json
{
  "success": false,
  "message": "Error description",
  "data": null,
  "errors": ["Detailed error message 1", "Detailed error message 2"],
  "timestamp": "2025-01-15T12:00:00Z",
  "traceId": "correlation-id-123"
}
```

#### Common HTTP Status Codes

- `200 OK`: Successful request
- `201 Created`: Resource created successfully
- `204 No Content`: Successful deletion
- `400 Bad Request`: Invalid request data
- `401 Unauthorized`: Authentication required
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Resource not found
- `409 Conflict`: Resource conflict (e.g., duplicate review)
- `429 Too Many Requests`: Rate limit exceeded
- `500 Internal Server Error`: Server error

## Advanced Features

### Health Monitoring

- **Endpoint**: `/health`
- **UI Dashboard**: `/health-ui` (development only)
- **Status Information**: Database connectivity, cache status, application health

### API Documentation

- **Swagger UI**: `/swagger` (includes interactive testing)
- **OpenAPI Specification**: Available in JSON format
- **Authentication Testing**: Built-in auth testing in Swagger

### Performance Features

- **Caching**: Automatic caching of frequently accessed data
- **Pagination**: Efficient handling of large datasets
- **Rate Limiting**: API abuse prevention
- **Optimized Queries**: Database performance optimization

### Monitoring and Logging

- **Request Logging**: All API requests logged with timing
- **Error Tracking**: Comprehensive error logging with context
- **Performance Metrics**: Response time monitoring
- **Health Checks**: Automated system health monitoring

## Troubleshooting

### Common Issues

#### Login/Authentication Issues

**Problem**: Cannot log in or access protected features
**Solutions:**

1. Verify username/password combination
2. Check if account is locked (5 failed attempts)
3. Clear browser cookies and cache
4. Try registering a new account

#### Book Creation/Editing Issues

**Problem**: Form validation errors or submission fails
**Solutions:**

1. Check all required fields are filled
2. Verify publication year is between 1000-2100
3. Ensure title doesn't exceed 200 characters
4. Check author name is under 100 characters

#### Review Issues

**Problem**: Cannot create or edit reviews
**Solutions:**

1. Ensure you're logged in
2. Check if you already reviewed this book (one review per user)
3. Verify review content is 10-2000 characters
4. Make sure rating is selected (1-5 stars)

#### API Issues

**Problem**: API calls failing or returning errors
**Solutions:**

1. Check API endpoint URL formatting
2. Verify authentication status
3. Check rate limiting (100 requests/minute)
4. Validate request body format and required fields
5. Review error messages in response body

#### Performance Issues

**Problem**: Slow page loading or timeouts
**Solutions:**

1. Check internet connection
2. Clear browser cache
3. Try different browser
4. Check server status at `/health` endpoint

### Getting Help

#### Error Information

When reporting issues, include:

- **Trace ID**: Found in API responses and error pages
- **Browser/Client**: Type and version
- **Steps to Reproduce**: Detailed action sequence
- **Expected vs Actual**: What should happen vs what happens
- **Screenshots**: Visual evidence of the issue

#### Support Channels

- **GitHub Issues**: Report bugs and feature requests
- **Documentation**: Check README.md for setup issues
- **Health Status**: Monitor system status at `/health`

#### Development/Testing

For development purposes:

- **Health Dashboard**: `/health-ui` shows system status
- **API Documentation**: `/swagger` for API testing
- **Log Files**: Check `logs/` directory for detailed error information
