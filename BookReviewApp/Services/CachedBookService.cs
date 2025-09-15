using AutoMapper;
using BookReviewApp.Common;
using BookReviewApp.Models.ViewModels;
using BookReviewApp.Models.ViewModels.Api;
using BookReviewApp.Services.Caching;
using BookReviewApp.Services.Interfaces;

namespace BookReviewApp.Services
{
    public class CachedBookService : IBookService
    {
        private readonly IBookService _bookService;
        private readonly ICacheService _cacheService;

        public CachedBookService(IBookService bookService, ICacheService cacheService)
        {
            _bookService = bookService;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync(string? genre = null, int? year = null, double? minRating = null)
        {
            var cacheKey = CacheKeys.GetBooksCacheKey(genre, year, minRating);
            return await _cacheService.GetOrSetAsync(cacheKey,
                () => _bookService.GetAllBooksAsync(genre, year, minRating),
                TimeSpan.FromMinutes(15));
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            var cacheKey = CacheKeys.GetBookCacheKey(id);
            return await _cacheService.GetOrSetAsync(cacheKey,
                () => _bookService.GetBookByIdAsync(id)!,
                TimeSpan.FromMinutes(30));
        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto)
        {
            var result = await _bookService.CreateBookAsync(createBookDto);

            // Invalidate related caches
            await _cacheService.RemoveByPatternAsync("books_.*");
            await _cacheService.RemoveAsync(CacheKeys.BookGenres);
            await _cacheService.RemoveAsync(CacheKeys.BookYears);

            return result;
        }

        public async Task<BookDto?> UpdateBookAsync(int id, CreateBookDto updateBookDto)
        {
            var result = await _bookService.UpdateBookAsync(id, updateBookDto);

            if (result != null)
            {
                // Invalidate related caches
                await _cacheService.RemoveAsync(CacheKeys.GetBookCacheKey(id));
                await _cacheService.RemoveByPatternAsync("books_.*");
                await _cacheService.RemoveByPatternAsync($"book_details_{id}_.*");
            }

            return result;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var result = await _bookService.DeleteBookAsync(id);

            if (result)
            {
                // Invalidate related caches
                await _cacheService.RemoveAsync(CacheKeys.GetBookCacheKey(id));
                await _cacheService.RemoveByPatternAsync("books_.*");
                await _cacheService.RemoveByPatternAsync($"book_details_{id}_.*");
                await _cacheService.RemoveByPatternAsync($"reviews_book_{id}");
            }

            return result;
        }

        public Task<bool> BookExistsAsync(int id)
        {
            return _bookService.BookExistsAsync(id);
        }

        public Task<BookListViewModel> GetBookListViewModelAsync(string? searchTerm = null, string? genre = null, int? year = null, double? minRating = null, string sortBy = "Title", string sortOrder = "asc", int page = 1, int pageSize = 10)
        {
            // Don't cache paginated results as they change frequently
            return _bookService.GetBookListViewModelAsync(searchTerm, genre, year, minRating, sortBy, sortOrder, page, pageSize);
        }

        public Task<BookDetailsViewModel> GetBookDetailsViewModelAsync(int id, string? userId = null)
        {
            // Don't cache user-specific data
            return _bookService.GetBookDetailsViewModelAsync(id, userId);
        }

        public async Task<IEnumerable<string>> GetGenresAsync()
        {
            return await _cacheService.GetOrSetAsync(CacheKeys.BookGenres,
                () => _bookService.GetGenresAsync(),
                TimeSpan.FromHours(2));
        }

        public async Task<IEnumerable<int>> GetPublishedYearsAsync()
        {
            return await _cacheService.GetOrSetAsync(CacheKeys.BookYears,
                () => _bookService.GetPublishedYearsAsync(),
                TimeSpan.FromHours(2));
        }
    }
}