using BookReviewApp.Common;
using BookReviewApp.Models.ViewModels.Api;
using BookReviewApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookReviewApp.Controllers.Api
{
    [Route("api/[controller]")]
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
        public async Task<ActionResult<ReviewDto>> CreateReview([FromBody] CreateReviewDto createReviewDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return HandleUnauthorized<ReviewDto>();
                }

                var bookExists = await _bookService.BookExistsAsync(createReviewDto.BookId);
                if (!bookExists)
                {
                    return HandleBadRequest<ReviewDto>(ErrorMessages.BookDoesNotExist);
                }

                var hasReviewed = await _reviewService.UserHasReviewedBookAsync(createReviewDto.BookId, userId);
                if (hasReviewed)
                {
                    return HandleConflict<ReviewDto>(ErrorMessages.BookAlreadyReviewed);
                }

                var review = await _reviewService.CreateReviewAsync(createReviewDto, userId);
                return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
            }
            catch (InvalidOperationException ex)
            {
                return HandleConflict<ReviewDto>(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return HandleBadRequest<ReviewDto>(ex.Message);
            }
            catch (Exception ex)
            {
                return HandleException<ReviewDto>(ex, "creating review");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetReview(int id)
        {
            try
            {
                var review = await _reviewService.GetReviewByIdAsync(id);

                if (review == null)
                {
                    return HandleNotFound<ReviewDto>("Review", id);
                }

                return Ok(review);
            }
            catch (Exception ex)
            {
                return HandleException<ReviewDto>(ex, $"fetching review {id}");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ReviewDto>> UpdateReview(int id, [FromBody] CreateReviewDto updateReviewDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return HandleUnauthorized<ReviewDto>();
                }

                var review = await _reviewService.UpdateReviewAsync(id, updateReviewDto, userId);

                if (review == null)
                {
                    return HandleNotFound<ReviewDto>("Review", id);
                }

                return Ok(review);
            }
            catch (UnauthorizedAccessException ex)
            {
                return HandleForbidden<ReviewDto>(ex.Message);
            }
            catch (Exception ex)
            {
                return HandleException<ReviewDto>(ex, $"updating review {id}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return HandleUnauthorized();
                }

                var result = await _reviewService.DeleteReviewAsync(id, userId);

                if (!result)
                {
                    return HandleNotFound("Review", id);
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return HandleForbidden(ex.Message);
            }
            catch (Exception ex)
            {
                return HandleException(ex, $"deleting review {id}");
            }
        }

        [HttpPost("{id}/vote")]
        [Authorize]
        public async Task<IActionResult> VoteOnReview(int id, [FromBody] bool isUpvote)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return HandleUnauthorized();
                }

                var reviewExists = await _reviewService.ReviewExistsAsync(id);
                if (!reviewExists)
                {
                    return HandleNotFound("Review", id);
                }

                var result = await _reviewService.VoteOnReviewAsync(id, userId, isUpvote);

                if (!result)
                {
                    return HandleBadRequest("Failed to vote on review.");
                }

                return Ok(new { message = SuccessMessages.VoteRecorded });
            }
            catch (InvalidOperationException ex)
            {
                return HandleBadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return HandleException(ex, $"voting on review {id}");
            }
        }
    }
}