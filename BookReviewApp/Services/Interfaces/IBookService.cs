using BookReviewApp.Models.ViewModels;
using BookReviewApp.Models.ViewModels.Api;

namespace BookReviewApp.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllBooksAsync(string? genre = null, int? year = null, double? minRating = null);
        Task<BookDto?> GetBookByIdAsync(int id);
        Task<BookDto> CreateBookAsync(CreateBookDto createBookDto);
        Task<BookDto?> UpdateBookAsync(int id, CreateBookDto updateBookDto);
        Task<bool> DeleteBookAsync(int id);
        Task<bool> BookExistsAsync(int id);

        Task<BookListViewModel> GetBookListViewModelAsync(
            string? searchTerm = null,
            string? genre = null,
            int? year = null,
            double? minRating = null,
            string sortBy = "Title",
            string sortOrder = "asc",
            int page = 1,
            int pageSize = 10);

        Task<BookDetailsViewModel> GetBookDetailsViewModelAsync(int id, string? userId = null);
        Task<IEnumerable<string>> GetGenresAsync();
        Task<IEnumerable<int>> GetPublishedYearsAsync();
    }
}