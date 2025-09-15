using BookReviewApp.Models.Api;
using BookReviewApp.Models.ViewModels.Api;
using BookReviewApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Controllers.Api
{
    public class BooksController : BaseController
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService, ILogger<BooksController> logger)
            : base(logger)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// Get all books with optional filtering
        /// </summary>
        /// <param name="genre">Filter by genre</param>
        /// <param name="year">Filter by publication year</param>
        /// <param name="minRating">Filter by minimum rating</param>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Number of items per page</param>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponse<BookDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<PaginatedResponse<BookDto>>> GetBooks(
    [FromQuery] string? genre = null,
    [FromQuery] int? year = null,
    [FromQuery] double? minRating = null,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var allBooks = await _bookService.GetAllBooksAsync(genre, year, minRating);
                var totalCount = allBooks.Count();
                var books = allBooks.Skip((page - 1) * pageSize).Take(pageSize);

                var response = PaginatedResponse<BookDto>.CreateResponse(books, page, pageSize, totalCount);
                response.TraceId = HttpContext.TraceIdentifier;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching books, TraceId: {TraceId}", HttpContext.TraceIdentifier);

                var errorResponse = ApiResponse<object>.ErrorResponse(
                    "An internal server error occurred",
                    new List<string> { ex.Message });
                errorResponse.TraceId = HttpContext.TraceIdentifier;

                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Get book by ID
        /// </summary>
        /// <param name="id">Book ID</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<BookDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<BookDto>>> GetBook(int id)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);

                if (book == null)
                {
                    return NotFoundResponse<BookDto>("Book", id);
                }

                return Success(book, "Book retrieved successfully");
            }
            catch (Exception ex)
            {
                return HandleException<BookDto>(ex, $"fetching book {id}");
            }
        }

        /// <summary>
        /// Create a new book
        /// </summary>
        /// <param name="createBookDto">Book creation data</param>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<BookDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<BookDto>>> CreateBook([FromBody] CreateBookDto createBookDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ValidationError<BookDto>();
                }

                var book = await _bookService.CreateBookAsync(createBookDto);
                var response = Success(book, "Book created successfully");

                return CreatedAtAction(nameof(GetBook), new { id = book.Id }, response.Value);
            }
            catch (Exception ex)
            {
                return HandleException<BookDto>(ex, "creating book");
            }
        }

        /// <summary>
        /// Update an existing book
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <param name="updateBookDto">Book update data</param>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<BookDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<BookDto>>> UpdateBook(int id, [FromBody] CreateBookDto updateBookDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ValidationError<BookDto>();
                }

                var book = await _bookService.UpdateBookAsync(id, updateBookDto);

                if (book == null)
                {
                    return NotFoundResponse<BookDto>("Book", id);
                }

                return Success(book, "Book updated successfully");
            }
            catch (Exception ex)
            {
                return HandleException<BookDto>(ex, $"updating book {id}");
            }
        }

        /// <summary>
        /// Delete a book
        /// </summary>
        /// <param name="id">Book ID</param>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var result = await _bookService.DeleteBookAsync(id);

                if (!result)
                {
                    return Error("Book not found", 404);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex, $"deleting book {id}");
            }
        }
    }
}