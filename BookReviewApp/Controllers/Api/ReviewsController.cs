using BookReviewApp.Common;
using BookReviewApp.Models.Api;
using BookReviewApp.Models.ViewModels.Api;
using BookReviewApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookReviewApp.Controllers.Api
{
    public class ReviewsController : BaseController
    {
        private readonly IReviewService _reviewService;
        private readonly IBookService _bookService;

        public ReviewsController(
            IReviewService reviewService,
            IBookService bookService,
            ILogger<ReviewsController> logger)
            : base(logger)
        {
            _reviewService = reviewService;
            _bookService = bookService;
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<ReviewDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<ReviewDto>>> CreateReview([FromBody] CreateReviewDto createReviewDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ValidationError<ReviewDto>();
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Error<ReviewDto>("User not authenticated", 401);
                }

                var bookExists = await _bookService.BookExistsAsync(createReviewDto.BookId);
                if (!bookExists)
                {
                    return Error<ReviewDto>(ErrorMessages.BookDoesNotExist, 400);
                }

                var hasReviewed = await _reviewService.UserHasReviewedBookAsync(createReviewDto.BookId, userId);
                if (hasReviewed)
                {
                    return Error<ReviewDto>(ErrorMessages.BookAlreadyReviewed, 409);
                }

                var review = await _reviewService.CreateReviewAsync(createReviewDto, userId);
                var response = Success(review, "Review created successfully");

                return CreatedAtAction(nameof(GetReview), new { id = review.Id }, response.Value);
            }
            catch (InvalidOperationException ex)
            {
                return Error<ReviewDto>(ex.Message, 409);
            }
            catch (ArgumentException ex)
            {
                return Error<ReviewDto>(ex.Message, 400);
            }
            catch (Exception ex)
            {
                return HandleException<ReviewDto>(ex, "creating review");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ReviewDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<ReviewDto>>> GetReview(int id)
        {
            try
            {
                var review = await _reviewService.GetReviewByIdAsync(id);

                if (review == null)
                {
                    return NotFoundResponse<ReviewDto>("Review", id);
                }

                return Success(review, "Review retrieved successfully");
            }
            catch (Exception ex)
            {
                return HandleException<ReviewDto>(ex, $"fetching review {id}");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<ReviewDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<ReviewDto>>> UpdateReview(int id, [FromBody] CreateReviewDto updateReviewDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ValidationError<ReviewDto>();
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Error<ReviewDto>("User not authenticated", 401);
                }

                var review = await _reviewService.UpdateReviewAsync(id, updateReviewDto, userId);

                if (review == null)
                {
                    return NotFoundResponse<ReviewDto>("Review", id);
                }

                return Success(review, "Review updated successfully");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Error<ReviewDto>(ex.Message, 403);
            }
            catch (Exception ex)
            {
                return HandleException<ReviewDto>(ex, $"updating review {id}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Error("User not authenticated", 401);
                }

                var result = await _reviewService.DeleteReviewAsync(id, userId);

                if (!result)
                {
                    return Error("Review not found", 404);
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Error(ex.Message, 403);
            }
            catch (Exception ex)
            {
                return HandleException(ex, $"deleting review {id}");
            }
        }

        [HttpPost("{id}/vote")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> VoteOnReview(int id, [FromBody] bool isUpvote)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Error("User not authenticated", 401);
                }

                var reviewExists = await _reviewService.ReviewExistsAsync(id);
                if (!reviewExists)
                {
                    return Error("Review not found", 404);
                }

                var result = await _reviewService.VoteOnReviewAsync(id, userId, isUpvote);

                if (!result)
                {
                    return Error("Failed to vote on review", 400);
                }

                return Success(SuccessMessages.VoteRecorded);
            }
            catch (InvalidOperationException ex)
            {
                return Error(ex.Message, 400);
            }
            catch (Exception ex)
            {
                return HandleException(ex, $"voting on review {id}");
            }
        }
    }
}