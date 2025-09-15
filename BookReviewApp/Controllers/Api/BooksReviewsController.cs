using BookReviewApp.Models.ViewModels.Api;
using BookReviewApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Controllers.Api
{
    [Route("api/books/{bookId}/reviews")]
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
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetBookReviews(int bookId)
        {
            try
            {
                var bookExists = await _bookService.BookExistsAsync(bookId);
                if (!bookExists)
                {
                    return HandleNotFound<IEnumerable<ReviewDto>>("Book", bookId);
                }

                var reviews = await _reviewService.GetReviewsByBookIdAsync(bookId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<ReviewDto>>(ex, $"fetching reviews for book {bookId}");
            }
        }
    }
}