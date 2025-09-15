using BookReviewApp.Models.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BookReviewApp.Controllers.Api
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [EnableRateLimiting("ApiPolicy")]
    public abstract class BaseController : ControllerBase
    {
        protected readonly ILogger _logger;

        protected BaseController(ILogger logger)
        {
            _logger = logger;
        }

        protected ActionResult<ApiResponse<T>> Success<T>(T data, string message = "Operation completed successfully")
        {
            var response = ApiResponse<T>.SuccessResponse(data, message);
            response.TraceId = HttpContext.TraceIdentifier;
            return Ok(response);
        }

        protected ActionResult<PaginatedResponse<T>> Success<T>(
            IEnumerable<T> data,
            int currentPage,
            int pageSize,
            int totalCount,
            string message = "Data retrieved successfully")
        {
            var response = PaginatedResponse<T>.CreateResponse(data, currentPage, pageSize, totalCount, message);
            response.TraceId = HttpContext.TraceIdentifier;
            return Ok(response);
        }

        protected ActionResult<ApiResponse<T>> Error<T>(string message, int statusCode = 400, List<string>? errors = null)
        {
            var response = ApiResponse<T>.ErrorResponse(message, errors);
            response.TraceId = HttpContext.TraceIdentifier;

            _logger.LogWarning("API Error: {Message}, TraceId: {TraceId}", message, HttpContext.TraceIdentifier);

            return StatusCode(statusCode, response);
        }

        protected ActionResult<ApiResponse<T>> HandleException<T>(Exception ex, string context = "processing request")
        {
            var message = $"An error occurred while {context}";
            _logger.LogError(ex, "{Message}, TraceId: {TraceId}", message, HttpContext.TraceIdentifier);

            var response = ApiResponse<T>.ErrorResponse(
                "An internal server error occurred",
                new List<string> { ex.Message });
            response.TraceId = HttpContext.TraceIdentifier;

            return StatusCode(500, response);
        }

        protected ActionResult<ApiResponse<T>> NotFoundResponse<T>(string entityName, object id)
        {
            return Error<T>($"{entityName} with ID {id} not found", 404);
        }

        protected ActionResult<ApiResponse<T>> ValidationError<T>(string message = "Validation failed")
        {
            var errors = new List<string>();

            if (!ModelState.IsValid)
            {
                errors.AddRange(ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
            }

            return Error<T>(message, 400, errors);
        }

        // Non-generic versions for methods that don't return data
        protected IActionResult Success(string message = "Operation completed successfully")
        {
            var response = new
            {
                Success = true,
                Message = message,
                Timestamp = DateTime.UtcNow,
                TraceId = HttpContext.TraceIdentifier
            };
            return Ok(response);
        }

        protected IActionResult Error(string message, int statusCode = 400)
        {
            var response = new
            {
                Success = false,
                Message = message,
                Timestamp = DateTime.UtcNow,
                TraceId = HttpContext.TraceIdentifier
            };

            _logger.LogWarning("API Error: {Message}, TraceId: {TraceId}", message, HttpContext.TraceIdentifier);
            return StatusCode(statusCode, response);
        }

        protected IActionResult HandleException(Exception ex, string context = "processing request")
        {
            var message = $"An error occurred while {context}";
            _logger.LogError(ex, "{Message}, TraceId: {TraceId}", message, HttpContext.TraceIdentifier);

            return StatusCode(500, new
            {
                Success = false,
                Message = "An internal server error occurred",
                Timestamp = DateTime.UtcNow,
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }
}