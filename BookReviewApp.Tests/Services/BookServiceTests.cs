using BookReviewApp.Services;
using BookReviewApp.Data.Repositories.Interfaces;

namespace BookReviewApp.Tests.Services
{
    public class BookServiceTests : IDisposable
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly BookService _bookService;

        public BookServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _bookService = new BookService(_mockUnitOfWork.Object);
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

        public void Dispose()
        {
            // Cleanup if needed
        }
    }
}