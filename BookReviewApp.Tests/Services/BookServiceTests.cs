using BookReviewApp.Services;
using BookReviewApp.Data.Repositories.Interfaces;

namespace BookReviewApp.Tests.Services
{
    public class BookServiceTests : IDisposable
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly BookService _bookService;
        private readonly ApplicationDbContext _context;

        public BookServiceTests()
        {
            _context = TestDbContext.CreateInMemoryContext();
            TestDbContext.SeedTestData(_context);

            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();

            _bookService = new BookService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task BookExistsAsync_ShouldReturnTrue_WhenBookExists()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.Books.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Book, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _bookService.BookExistsAsync(1);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task BookExistsAsync_ShouldReturnFalse_WhenBookDoesNotExist()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.Books.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Book, bool>>>()))
                .ReturnsAsync(false);

            // Act
            var result = await _bookService.BookExistsAsync(999);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task CreateBookAsync_ShouldCallAddAndSave_WithValidData()
        {
            // Arrange
            var createBookDto = new CreateBookDto
            {
                Title = "Test Book",
                Author = "Test Author",
                Genre = "Fiction",
                PublishedYear = 2023
            };

            var expectedBook = new Book { Id = 1, Title = "Test Book" };
            var expectedDto = new BookDto { Id = 1, Title = "Test Book" };

            _mockMapper.Setup(x => x.Map<Book>(createBookDto)).Returns(expectedBook);
            _mockMapper.Setup(x => x.Map<BookDto>(expectedBook)).Returns(expectedDto);
            _mockUnitOfWork.Setup(x => x.Books.AddAsync(It.IsAny<Book>())).ReturnsAsync(expectedBook);
            _mockUnitOfWork.Setup(x => x.SaveAsync()).ReturnsAsync(1);

            // Act
            var result = await _bookService.CreateBookAsync(createBookDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            _mockUnitOfWork.Verify(x => x.Books.AddAsync(It.IsAny<Book>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}