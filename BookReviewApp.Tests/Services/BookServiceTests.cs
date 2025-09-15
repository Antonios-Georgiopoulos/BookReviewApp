using BookReviewApp.Services;

namespace BookReviewApp.Tests.Services
{
    public class BookServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly BookService _bookService;

        public BookServiceTests()
        {
            _context = TestDbContext.CreateInMemoryContext();
            TestDbContext.SeedTestData(_context);
            _bookService = new BookService(_context);
        }

        [Fact]
        public async Task GetAllBooksAsync_ShouldReturnAllBooks_WhenNoFiltersApplied()
        {
            // Act
            var result = await _bookService.GetAllBooksAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(b => b.Title == "Test Book 1");
            result.Should().Contain(b => b.Title == "Test Book 2");
        }

        [Fact]
        public async Task GetAllBooksAsync_ShouldFilterByGenre_WhenGenreFilterApplied()
        {
            // Act
            var result = await _bookService.GetAllBooksAsync(genre: "Fiction");

            // Assert
            result.Should().HaveCount(1);
            result.First().Title.Should().Be("Test Book 1");
        }

        [Fact]
        public async Task GetBookByIdAsync_ShouldReturnBook_WhenBookExists()
        {
            // Act
            var result = await _bookService.GetBookByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Title.Should().Be("Test Book 1");
            result.AverageRating.Should().Be(4.5); // (5 + 4) / 2 = 4.5
            result.ReviewCount.Should().Be(2);
        }

        [Fact]
        public async Task GetBookByIdAsync_ShouldReturnNull_WhenBookDoesNotExist()
        {
            // Act
            var result = await _bookService.GetBookByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateBookAsync_ShouldCreateNewBook_WithValidData()
        {
            // Arrange
            var createBookDto = new CreateBookDto
            {
                Title = "New Test Book",
                Author = "New Author",
                Genre = "Science Fiction",
                PublishedYear = 2024
            };

            // Act
            var result = await _bookService.CreateBookAsync(createBookDto);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("New Test Book");
            result.Author.Should().Be("New Author");
            result.Genre.Should().Be("Science Fiction");
            result.PublishedYear.Should().Be(2024);

            // Verify in database
            var bookInDb = await _context.Books.FindAsync(result.Id);
            bookInDb.Should().NotBeNull();
            bookInDb!.Title.Should().Be("New Test Book");
        }

        [Fact]
        public async Task BookExistsAsync_ShouldReturnTrue_WhenBookExists()
        {
            // Act
            var result = await _bookService.BookExistsAsync(1);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task BookExistsAsync_ShouldReturnFalse_WhenBookDoesNotExist()
        {
            // Act
            var result = await _bookService.BookExistsAsync(999);

            // Assert
            result.Should().BeFalse();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}