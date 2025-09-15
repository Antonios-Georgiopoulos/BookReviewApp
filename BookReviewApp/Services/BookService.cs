using BookReviewApp.Data;
using BookReviewApp.Models.Domain;
using BookReviewApp.Models.ViewModels;
using BookReviewApp.Models.ViewModels.Api;
using BookReviewApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;

        public BookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync(string? genre = null, int? year = null, double? minRating = null)
        {
            var query = _context.Books
                .Include(b => b.Reviews)
                .AsQueryable();

            if (!string.IsNullOrEmpty(genre))
            {
                query = query.Where(b => b.Genre.Equals(genre, StringComparison.CurrentCultureIgnoreCase));
            }

            if (year.HasValue)
            {
                query = query.Where(b => b.PublishedYear == year.Value);
            }

            var books = await query.ToListAsync();

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
                bookDtos = [.. bookDtos.Where(b => b.AverageRating >= minRating.Value)];
            }

            return bookDtos;
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            var book = await _context.Books
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == id);

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

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

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
            var book = await _context.Books.FindAsync(id);
            if (book == null) return null;

            book.Title = updateBookDto.Title;
            book.Author = updateBookDto.Author;
            book.PublishedYear = updateBookDto.PublishedYear;
            book.Genre = updateBookDto.Genre;

            await _context.SaveChangesAsync();

            return await GetBookByIdAsync(id);
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BookExistsAsync(int id)
        {
            return await _context.Books.AnyAsync(b => b.Id == id);
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
            var query = _context.Books
                .Include(b => b.Reviews)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(b => b.Title.Contains(searchTerm) || b.Author.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(genre))
            {
                query = query.Where(b => b.Genre == genre);
            }

            if (year.HasValue)
            {
                query = query.Where(b => b.PublishedYear == year.Value);
            }

            var books = await query.ToListAsync();

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
                bookViewModels = [.. bookViewModels.Where(b => b.AverageRating >= minRating.Value)];
            }

            bookViewModels = sortBy.ToLower() switch
            {
                "author" => sortOrder == "desc" ? [.. bookViewModels.OrderByDescending(b => b.Author)] : [.. bookViewModels.OrderBy(b => b.Author)],
                "publishedyear" => sortOrder == "desc" ? [.. bookViewModels.OrderByDescending(b => b.PublishedYear)] : [.. bookViewModels.OrderBy(b => b.PublishedYear)],
                "averagerating" => sortOrder == "desc" ? [.. bookViewModels.OrderByDescending(b => b.AverageRating)] : [.. bookViewModels.OrderBy(b => b.AverageRating)],
                "datecreated" => sortOrder == "desc" ? [.. bookViewModels.OrderByDescending(b => b.DateCreated)] : [.. bookViewModels.OrderBy(b => b.DateCreated)],
                _ => sortOrder == "desc" ? [.. bookViewModels.OrderByDescending(b => b.Title)] : [.. bookViewModels.OrderBy(b => b.Title)]
            };

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
            var book = await _context.Books
                .Include(b => b.Reviews)
                    .ThenInclude(r => r.User)
                .Include(b => b.Reviews)
                    .ThenInclude(r => r.ReviewVotes)
                .FirstOrDefaultAsync(b => b.Id == id) ?? throw new ArgumentException($"Book with id {id} not found");
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
            return await _context.Books
                .Select(b => b.Genre)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();
        }

        public async Task<IEnumerable<int>> GetPublishedYearsAsync()
        {
            return await _context.Books
                .Select(b => b.PublishedYear)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();
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