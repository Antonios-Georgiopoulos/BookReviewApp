using BookReviewApp.Controllers.Api;
using BookReviewApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookReviewApp.Tests.Controllers
{
    public class BooksControllerTests
    {
        private readonly Mock<IBookService> _mockBookService;
        private readonly Mock<ILogger<BooksController>> _mockLogger;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            _mockBookService = new Mock<IBookService>();
            _mockLogger = new Mock<ILogger<BooksController>>();
            _controller = new BooksController(_mockBookService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetBooks_ShouldReturnOkResult_WithBooks()
        {
            // Arrange
            var books = new List<BookDto>
            {
                new() { Id = 1, Title = "Test Book 1", Author = "Author 1" },
                new() { Id = 2, Title = "Test Book 2", Author = "Author 2" }
            };
            _mockBookService.Setup(s => s.GetAllBooksAsync(null, null, null))
                .ReturnsAsync(books);

            // Act
            var result = await _controller.GetBooks();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(books);
        }

        [Fact]
        public async Task GetBook_ShouldReturnOkResult_WhenBookExists()
        {
            // Arrange
            var book = new BookDto { Id = 1, Title = "Test Book", Author = "Test Author" };
            _mockBookService.Setup(s => s.GetBookByIdAsync(1))
                .ReturnsAsync(book);

            // Act
            var result = await _controller.GetBook(1);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(book);
        }

        [Fact]
        public async Task GetBook_ShouldReturnNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            _mockBookService.Setup(s => s.GetBookByIdAsync(1))
                .ReturnsAsync((BookDto?)null);

            // Act
            var result = await _controller.GetBook(1);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}