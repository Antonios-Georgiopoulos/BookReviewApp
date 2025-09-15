using BookReviewApp.Common;
using BookReviewApp.Models.ViewModels;
using BookReviewApp.Models.ViewModels.Api;
using BookReviewApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookReviewApp.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IReviewService _reviewService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(
            IBookService bookService,
            IReviewService reviewService,
            ILogger<BooksController> logger)
        {
            _bookService = bookService;
            _reviewService = reviewService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(
            string? searchTerm = null,
            string? genre = null,
            int? year = null,
            double? minRating = null,
            string sortBy = "Title",
            string sortOrder = "asc",
            int page = 1)
        {
            try
            {
                var viewModel = await _bookService.GetBookListViewModelAsync(
                    searchTerm, genre, year, minRating, sortBy, sortOrder, page, 10);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching books list");
                TempData[TempDataKeys.Error] = "Παρουσιάστηκε σφάλμα κατά τη φόρτωση των βιβλίων.";
                return View(new BookListViewModel());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var viewModel = await _bookService.GetBookDetailsViewModelAsync(id, userId);

                return View(viewModel);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching book details for {BookId}", id);
                TempData[TempDataKeys.Error] = "Παρουσιάστηκε σφάλμα κατά τη φόρτωση των στοιχείων του βιβλίου.";
                return RedirectToAction(ActionNames.Index);
            }
        }

        [Authorize]
        public IActionResult Create()
        {
            return View(new BookViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(BookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var createBookDto = new CreateBookDto
                {
                    Title = model.Title,
                    Author = model.Author,
                    PublishedYear = model.PublishedYear,
                    Genre = model.Genre
                };

                var book = await _bookService.CreateBookAsync(createBookDto);
                TempData[TempDataKeys.Success] = SuccessMessages.BookCreated;
                return RedirectToAction(ActionNames.Details, new { id = book.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating book");
                ModelState.AddModelError("", "Παρουσιάστηκε σφάλμα κατά τη δημιουργία του βιβλίου.");
                return View(model);
            }
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);
                if (book == null)
                {
                    return NotFound();
                }

                var model = new BookViewModel
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    PublishedYear = book.PublishedYear,
                    Genre = book.Genre,
                    DateCreated = book.DateCreated,
                    AverageRating = book.AverageRating,
                    ReviewCount = book.ReviewCount
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching book for edit {BookId}", id);
                TempData[TempDataKeys.Error] = "Παρουσιάστηκε σφάλμα κατά τη φόρτωση του βιβλίου για επεξεργασία.";
                return RedirectToAction(ActionNames.Index);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, BookViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var updateBookDto = new CreateBookDto
                {
                    Title = model.Title,
                    Author = model.Author,
                    PublishedYear = model.PublishedYear,
                    Genre = model.Genre
                };

                var book = await _bookService.UpdateBookAsync(id, updateBookDto);
                if (book == null)
                {
                    return NotFound();
                }

                TempData[TempDataKeys.Success] = SuccessMessages.BookUpdated;
                return RedirectToAction(ActionNames.Details, new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating book {BookId}", id);
                ModelState.AddModelError("", "Παρουσιάστηκε σφάλμα κατά την ενημέρωση του βιβλίου.");
                return View(model);
            }
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);
                if (book == null)
                {
                    return NotFound();
                }

                var model = new BookViewModel
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    PublishedYear = book.PublishedYear,
                    Genre = book.Genre,
                    DateCreated = book.DateCreated,
                    AverageRating = book.AverageRating,
                    ReviewCount = book.ReviewCount
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching book for delete {BookId}", id);
                TempData[TempDataKeys.Error] = "Παρουσιάστηκε σφάλμα κατά τη φόρτωση του βιβλίου για διαγραφή.";
                return RedirectToAction(ActionNames.Index);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _bookService.DeleteBookAsync(id);
                if (!result)
                {
                    TempData[TempDataKeys.Error] = "Το βιβλίο δεν βρέθηκε.";
                }
                else
                {
                    TempData[TempDataKeys.Success] = SuccessMessages.BookDeleted;
                }

                return RedirectToAction(ActionNames.Index);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting book {BookId}", id);
                TempData[TempDataKeys.Error] = "Παρουσιάστηκε σφάλμα κατά τη διαγραφή του βιβλίου.";
                return RedirectToAction(ActionNames.Index);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddReview(BookDetailsViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                var bookDetails = await _bookService.GetBookDetailsViewModelAsync(model.Book.Id, userId);
                bookDetails.NewReview = model.NewReview;
                return View(ActionNames.Details, bookDetails);
            }

            try
            {
                var createReviewDto = new CreateReviewDto
                {
                    Content = model.NewReview.Content,
                    Rating = model.NewReview.Rating,
                    BookId = model.Book.Id
                };

                await _reviewService.CreateReviewAsync(createReviewDto, userId);
                TempData[TempDataKeys.Success] = SuccessMessages.ReviewAdded;

                return RedirectToAction(ActionNames.Details, new { id = model.Book.Id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("NewReview.Content", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding review");
                ModelState.AddModelError("", "Παρουσιάστηκε σφάλμα κατά την προσθήκη της κριτικής.");
            }

            var reloadedBookDetails = await _bookService.GetBookDetailsViewModelAsync(model.Book.Id, userId);
            reloadedBookDetails.NewReview = model.NewReview;
            return View(ActionNames.Details, reloadedBookDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> VoteReview(int reviewId, bool isUpvote)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Δεν είστε συνδεδεμένος." });
            }

            try
            {
                var result = await _reviewService.VoteOnReviewAsync(reviewId, userId, isUpvote);
                if (result)
                {
                    return Json(new { success = true, message = SuccessMessages.VoteRecorded });
                }
                else
                {
                    return Json(new { success = false, message = "Αποτυχία καταγραφής ψήφου." });
                }
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while voting on review {ReviewId}", reviewId);
                return Json(new { success = false, message = "Παρουσιάστηκε σφάλμα κατά την ψηφοφορία." });
            }
        }
    }
}