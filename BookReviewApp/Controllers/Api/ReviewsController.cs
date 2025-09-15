using BookReviewApp.Models.ViewModels.Api;
using BookReviewApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookReviewApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IBookService _bookService;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(
            IReviewService reviewService,
            IBookService bookService,
            ILogger<ReviewsController> logger)
        {
            _reviewService = reviewService;
            _bookService = bookService;
            _logger = logger;
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
                    return Unauthorized("User not authenticated.");
                }

                var bookExists = await _bookService.BookExistsAsync(createReviewDto.BookId);
                if (!bookExists)
                {
                    return BadRequest("Book does not exist.");
                }

                var hasReviewed = await _reviewService.UserHasReviewedBookAsync(createReviewDto.BookId, userId);
                if (hasReviewed)
                {
                    return Conflict("User has already reviewed this book.");
                }

                var review = await _reviewService.CreateReviewAsync(createReviewDto, userId);
                return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating review");
                return StatusCode(500, "An error occurred while processing your request.");
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
                    return NotFound($"Review with ID {id} not found.");
                }

                return Ok(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching review {ReviewId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
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
                    return Unauthorized("User not authenticated.");
                }

                var review = await _reviewService.UpdateReviewAsync(id, updateReviewDto, userId);

                if (review == null)
                {
                    return NotFound($"Review with ID {id} not found.");
                }

                return Ok(review);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating review {ReviewId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
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
                    return Unauthorized("User not authenticated.");
                }

                var result = await _reviewService.DeleteReviewAsync(id, userId);

                if (!result)
                {
                    return NotFound($"Review with ID {id} not found.");
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting review {ReviewId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
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
                    return Unauthorized("User not authenticated.");
                }

                // Check if review exists
                var reviewExists = await _reviewService.ReviewExistsAsync(id);
                if (!reviewExists)
                {
                    return NotFound($"Review with ID {id} not found.");
                }

                var result = await _reviewService.VoteOnReviewAsync(id, userId, isUpvote);

                if (!result)
                {
                    return BadRequest("Failed to vote on review.");
                }

                return Ok(new { message = "Vote recorded successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while voting on review {ReviewId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}