using BookReviewApp.Models.ViewModels.Api;
using BookReviewApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookReviewApp.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
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

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var review = await _reviewService.GetReviewByIdAsync(id);
                if (review == null)
                {
                    return NotFound();
                }

                if (review.UserId != userId)
                {
                    return Forbid();
                }

                var model = new CreateReviewDto
                {
                    Content = review.Content,
                    Rating = review.Rating,
                    BookId = review.BookId
                };

                ViewBag.ReviewId = id;
                ViewBag.BookTitle = review.BookTitle;

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching review for edit {ReviewId}", id);
                TempData["Error"] = "Παρουσιάστηκε σφάλμα κατά τη φόρτωση της κριτικής για επεξεργασία.";
                return RedirectToAction("Index", "Books");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateReviewDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReviewId = id;
                ViewBag.BookTitle = await GetBookTitleAsync(model.BookId);
                return View(model);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var review = await _reviewService.UpdateReviewAsync(id, model, userId);
                if (review == null)
                {
                    return NotFound();
                }

                TempData["Success"] = "Η κριτική ενημερώθηκε επιτυχώς!";
                return RedirectToAction("Details", "Books", new { id = model.BookId });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating review {ReviewId}", id);
                ModelState.AddModelError("", "Παρουσιάστηκε σφάλμα κατά την ενημέρωση της κριτικής.");
                ViewBag.ReviewId = id;
                ViewBag.BookTitle = await GetBookTitleAsync(model.BookId);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int bookId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var result = await _reviewService.DeleteReviewAsync(id, userId);
                if (!result)
                {
                    TempData["Error"] = "Η κριτική δεν βρέθηκε.";
                }
                else
                {
                    TempData["Success"] = "Η κριτική διαγράφηκε επιτυχώς!";
                }

                return RedirectToAction("Details", "Books", new { id = bookId });
            }
            catch (UnauthorizedAccessException)
            {
                TempData["Error"] = "Δεν έχετε δικαίωμα να διαγράψετε αυτή την κριτική.";
                return RedirectToAction("Details", "Books", new { id = bookId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting review {ReviewId}", id);
                TempData["Error"] = "Παρουσιάστηκε σφάλμα κατά τη διαγραφή της κριτικής.";
                return RedirectToAction("Details", "Books", new { id = bookId });
            }
        }

        private async Task<string> GetBookTitleAsync(int bookId)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(bookId);
                return book?.Title ?? "Unknown Book";
            }
            catch
            {
                return "Unknown Book";
            }
        }
    }
}