using BookReviewApp.Data.Repositories.Interfaces;
using BookReviewApp.Models.Domain;
using BookReviewApp.Models.ViewModels;
using BookReviewApp.Models.ViewModels.Api;
using BookReviewApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookReviewApp.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync(string? genre = null, int? year = null, double? minRating = null)
        {
            var books = await _unitOfWork.Books.GetAllAsync(
                filter: b => (string.IsNullOrEmpty(genre) || b.Genre.Equals(genre, StringComparison.CurrentCultureIgnoreCase)) &&
                            (!year.HasValue || b.PublishedYear == year.Value),
                includeProperties: "Reviews"
            );

            var bookDtos = books.Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                PublishedYear = b.PublishedYear,
                Genre = b.Genre,
                DateCreated = b.DateCreated,
                ReviewCount = b.Reviews.Count,
                AverageRating = b.Reviews.Count != 0 ? Math.Round(b.Reviews.Average(r => r.Rating), 2) : 0
            }).ToList();

            if (minRating.HasValue)
            {
                bookDtos = bookDtos.Where(b => b.AverageRating >= minRating.Value).ToList();
            }

            return bookDtos;
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            var book = await _unitOfWork.Books.GetFirstOrDefaultAsync(
                filter: b => b.Id == id,
                includeProperties: "Reviews"
            );

            if (book == null) return null;

            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                PublishedYear = book.PublishedYear,
                Genre = book.Genre,
                DateCreated = book.DateCreated,
                ReviewCount = book.Reviews.Count,
                AverageRating = book.Reviews.Count != 0 ? Math.Round(book.Reviews.Average(r => r.Rating), 2) : 0
            };
        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto)
        {
            var book = new Book
            {
                Title = createBookDto.Title,
                Author = createBookDto.Author,
                PublishedYear = createBookDto.PublishedYear,
                Genre = createBookDto.Genre,
                DateCreated = DateTime.UtcNow
            };

            await _unitOfWork.Books.AddAsync(book);
            await _unitOfWork.SaveAsync();

            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                PublishedYear = book.PublishedYear,
                Genre = book.Genre,
                DateCreated = book.DateCreated,
                ReviewCount = 0,
                AverageRating = 0
            };
        }

        public async Task<BookDto?> UpdateBookAsync(int id, CreateBookDto updateBookDto)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null) return null;

            book.Title = updateBookDto.Title;
            book.Author = updateBookDto.Author;
            book.PublishedYear = updateBookDto.PublishedYear;
            book.Genre = updateBookDto.Genre;

            _unitOfWork.Books.Update(book);
            await _unitOfWork.SaveAsync();

            return await GetBookByIdAsync(id);
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null) return false;

            _unitOfWork.Books.Delete(book);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> BookExistsAsync(int id)
        {
            return await _unitOfWork.Books.ExistsAsync(b => b.Id == id);
        }

        public async Task<BookListViewModel> GetBookListViewModelAsync(
            string? searchTerm = null,
            string? genre = null,
            int? year = null,
            double? minRating = null,
            string sortBy = "Title",
            string sortOrder = "asc",
            int page = 1,
            int pageSize = 10)
        {
            var books = await _unitOfWork.Books.GetAllAsync(
                filter: b => (string.IsNullOrEmpty(searchTerm) || b.Title.Contains(searchTerm) || b.Author.Contains(searchTerm)) &&
                            (string.IsNullOrEmpty(genre) || b.Genre == genre) &&
                            (!year.HasValue || b.PublishedYear == year.Value),
                includeProperties: "Reviews"
            );

            var bookViewModels = books.Select(b => new BookViewModel
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                PublishedYear = b.PublishedYear,
                Genre = b.Genre,
                DateCreated = b.DateCreated,
                ReviewCount = b.Reviews.Count,
                AverageRating = b.Reviews.Count != 0 ? Math.Round(b.Reviews.Average(r => r.Rating), 2) : 0
            }).ToList();

            if (minRating.HasValue)
            {
                bookViewModels = bookViewModels.Where(b => b.AverageRating >= minRating.Value).ToList();
            }

            // Apply sorting
            bookViewModels = ApplySorting(bookViewModels, sortBy, sortOrder);

            var totalBooks = bookViewModels.Count;
            var totalPages = (int)Math.Ceiling(totalBooks / (double)pageSize);
            var paginatedBooks = bookViewModels.Skip((page - 1) * pageSize).Take(pageSize);

            return new BookListViewModel
            {
                Books = paginatedBooks,
                SearchTerm = searchTerm,
                SelectedGenre = genre,
                PublishedYear = year,
                MinRating = minRating,
                SortBy = sortBy,
                SortOrder = sortOrder,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalBooks = totalBooks,
                GenreOptions = await GetGenreSelectListAsync(),
                YearOptions = await GetYearSelectListAsync()
            };
        }

        public async Task<BookDetailsViewModel> GetBookDetailsViewModelAsync(int id, string? userId = null)
        {
            var book = await _unitOfWork.Books.GetFirstOrDefaultAsync(
                filter: b => b.Id == id,
                includeProperties: "Reviews,Reviews.User,Reviews.ReviewVotes"
            );

            if (book == null)
                throw new ArgumentException($"Book with id {id} not found");

            var bookViewModel = new BookViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                PublishedYear = book.PublishedYear,
                Genre = book.Genre,
                DateCreated = book.DateCreated,
                ReviewCount = book.Reviews.Count,
                AverageRating = book.Reviews.Count != 0 ? Math.Round(book.Reviews.Average(r => r.Rating), 2) : 0
            };

            var reviewViewModels = book.Reviews.Select(r => new ReviewViewModel
            {
                Id = r.Id,
                Content = r.Content,
                Rating = r.Rating,
                DateCreated = r.DateCreated,
                BookId = r.BookId,
                BookTitle = book.Title,
                UserId = r.UserId,
                UserName = r.User.UserName ?? "Unknown",
                UpvoteCount = r.ReviewVotes.Count(rv => rv.IsUpvote),
                DownvoteCount = r.ReviewVotes.Count(rv => !rv.IsUpvote),
                UserVote = !string.IsNullOrEmpty(userId) ?
                    r.ReviewVotes.FirstOrDefault(rv => rv.UserId == userId)?.IsUpvote : null
            }).OrderByDescending(r => r.DateCreated);

            var userHasReviewed = !string.IsNullOrEmpty(userId) &&
                book.Reviews.Any(r => r.UserId == userId);

            return new BookDetailsViewModel
            {
                Book = bookViewModel,
                Reviews = reviewViewModels,
                NewReview = new ReviewViewModel { BookId = id },
                CanAddReview = !string.IsNullOrEmpty(userId),
                UserHasReviewed = userHasReviewed
            };
        }

        public async Task<IEnumerable<string>> GetGenresAsync()
        {
            var books = await _unitOfWork.Books.GetAllAsync();
            return books.Select(b => b.Genre).Distinct().OrderBy(g => g);
        }

        public async Task<IEnumerable<int>> GetPublishedYearsAsync()
        {
            var books = await _unitOfWork.Books.GetAllAsync();
            return books.Select(b => b.PublishedYear).Distinct().OrderByDescending(y => y);
        }

        private static List<BookViewModel> ApplySorting(List<BookViewModel> books, string sortBy, string sortOrder)
        {
            return sortBy.ToLower() switch
            {
                "author" => sortOrder == "desc" ? books.OrderByDescending(b => b.Author).ToList() : books.OrderBy(b => b.Author).ToList(),
                "publishedyear" => sortOrder == "desc" ? books.OrderByDescending(b => b.PublishedYear).ToList() : books.OrderBy(b => b.PublishedYear).ToList(),
                "averagerating" => sortOrder == "desc" ? books.OrderByDescending(b => b.AverageRating).ToList() : books.OrderBy(b => b.AverageRating).ToList(),
                "datecreated" => sortOrder == "desc" ? books.OrderByDescending(b => b.DateCreated).ToList() : books.OrderBy(b => b.DateCreated).ToList(),
                _ => sortOrder == "desc" ? books.OrderByDescending(b => b.Title).ToList() : books.OrderBy(b => b.Title).ToList()
            };
        }

        private async Task<IEnumerable<SelectListItem>> GetGenreSelectListAsync()
        {
            var genres = await GetGenresAsync();
            return genres.Select(g => new SelectListItem { Value = g, Text = g });
        }

        private async Task<IEnumerable<SelectListItem>> GetYearSelectListAsync()
        {
            var years = await GetPublishedYearsAsync();
            return years.Select(y => new SelectListItem { Value = y.ToString(), Text = y.ToString() });
        }
    }
}