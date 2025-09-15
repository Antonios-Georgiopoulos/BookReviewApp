using AutoMapper;
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
        private readonly IMapper _mapper;

        public BookService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync(string? genre = null, int? year = null, double? minRating = null)
        {
            var books = await _unitOfWork.Books.GetAllAsync(
                filter: b => (string.IsNullOrEmpty(genre) || b.Genre.Equals(genre, StringComparison.CurrentCultureIgnoreCase)) &&
                            (!year.HasValue || b.PublishedYear == year.Value),
                includeProperties: "Reviews"
            );

            var bookDtos = _mapper.Map<List<BookDto>>(books);

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

            return book == null ? null : _mapper.Map<BookDto>(book);
        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto)
        {
            var book = _mapper.Map<Book>(createBookDto);
            book.DateCreated = DateTime.UtcNow;

            await _unitOfWork.Books.AddAsync(book);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<BookDto>(book);
        }

        public async Task<BookDto?> UpdateBookAsync(int id, CreateBookDto updateBookDto)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null) return null;

            _mapper.Map(updateBookDto, book);
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

            var bookViewModels = _mapper.Map<List<BookViewModel>>(books);

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

            var bookViewModel = _mapper.Map<BookViewModel>(book);
            var reviewViewModels = _mapper.Map<List<ReviewViewModel>>(book.Reviews);

            // Set user vote information
            if (!string.IsNullOrEmpty(userId))
            {
                foreach (var reviewVM in reviewViewModels)
                {
                    var review = book.Reviews.First(r => r.Id == reviewVM.Id);
                    reviewVM.UserVote = review.ReviewVotes.FirstOrDefault(rv => rv.UserId == userId)?.IsUpvote;
                }
            }

            var userHasReviewed = !string.IsNullOrEmpty(userId) &&
                book.Reviews.Any(r => r.UserId == userId);

            return new BookDetailsViewModel
            {
                Book = bookViewModel,
                Reviews = reviewViewModels.OrderByDescending(r => r.DateCreated),
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