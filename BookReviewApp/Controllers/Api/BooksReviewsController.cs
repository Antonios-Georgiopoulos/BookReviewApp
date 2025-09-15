using BookReviewApp.Models.Api;
using BookReviewApp.Models.ViewModels.Api;
using BookReviewApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Controllers.Api
{
    public class BooksReviewsController : BaseController
    {
        private readonly IReviewService _reviewService;
        private readonly IBookService _bookService;

        public BooksReviewsController(
            IReviewService reviewService,
            IBookService bookService,
            ILogger<BooksReviewsController> logger)
            : base(logger)
        {
            _reviewService = reviewService;
            _bookService = bookService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ReviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReviewDto>>>> GetBookReviews(int bookId)
        {
            try
            {
                var bookExists = await _bookService.BookExistsAsync(bookId);
                if (!bookExists)
                {
                    return NotFoundResponse<IEnumerable<ReviewDto>>("Book", bookId);
                }

                var reviews = await _reviewService.GetReviewsByBookIdAsync(bookId);
                return Success(reviews, "Reviews retrieved successfully");
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<ReviewDto>>(ex, $"fetching reviews for book {bookId}");
            }
        }
    }
}