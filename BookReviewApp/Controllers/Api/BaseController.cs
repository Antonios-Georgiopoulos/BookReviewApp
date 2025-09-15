using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Controllers.Api
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected readonly ILogger _logger;

        protected BaseController(ILogger logger)
        {
            _logger = logger;
        }

        protected ActionResult<T> HandleException<T>(Exception ex, string context = "")
        {
            var message = string.IsNullOrEmpty(context)
                ? "An error occurred while processing your request."
                : $"An error occurred while {context}";

            _logger.LogError(ex, message);
            return StatusCode(500, message);
        }

        protected ActionResult<T> HandleNotFound<T>(string entityName, object id)
        {
            var message = $"{entityName} with ID {id} not found.";
            return NotFound(message);
        }

        protected ActionResult<T> HandleBadRequest<T>(string message)
        {
            return BadRequest(message);
        }

        protected ActionResult<T> HandleConflict<T>(string message)
        {
            return Conflict(message);
        }

        protected ActionResult<T> HandleUnauthorized<T>(string message = "User not authenticated.")
        {
            return Unauthorized(message);
        }

        protected ActionResult<T> HandleForbidden<T>(string message)
        {
            return Forbid(message);
        }

        // Overloads for IActionResult (for Delete methods)
        protected IActionResult HandleException(Exception ex, string context = "")
        {
            var message = string.IsNullOrEmpty(context)
                ? "An error occurred while processing your request."
                : $"An error occurred while {context}";

            _logger.LogError(ex, message);
            return StatusCode(500, message);
        }

        protected IActionResult HandleNotFound(string entityName, object id)
        {
            var message = $"{entityName} with ID {id} not found.";
            return NotFound(message);
        }

        protected IActionResult HandleBadRequest(string message)
        {
            return BadRequest(message);
        }

        protected IActionResult HandleUnauthorized(string message = "User not authenticated.")
        {
            return Unauthorized(message);
        }

        protected IActionResult HandleForbidden(string message)
        {
            return Forbid(message);
        }
    }
}