using BookReviewApp.Models.ViewModels.Api;
using BookReviewApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Controllers.Api
{
    [Route("api/books/{bookId}/reviews")]
    [ApiController]
    public class BooksReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IBookService _bookService;
        private readonly ILogger<BooksReviewsController> _logger;

        public BooksReviewsController(
            IReviewService reviewService,
            IBookService bookService,
            ILogger<BooksReviewsController> logger)
        {
            _reviewService = reviewService;
            _bookService = bookService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetBookReviews(int bookId)
        {
            try
            {
                // Check if book exists
                var bookExists = await _bookService.BookExistsAsync(bookId);
                if (!bookExists)
                {
                    return NotFound($"Book with ID {bookId} not found.");
                }

                var reviews = await _reviewService.GetReviewsByBookIdAsync(bookId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching reviews for book {BookId}", bookId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}