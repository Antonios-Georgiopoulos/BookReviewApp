using BookReviewApp.Controllers.Api;
using BookReviewApp.Models.Api;
using BookReviewApp.Models.ViewModels.Api;
using BookReviewApp.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookReviewApp.Tests.Controllers
{
    public class BooksControllerMoreTests
    {
        private readonly Mock<IBookService> _bookService = new();
        private readonly Mock<ILogger<BooksController>> _logger = new();
        private readonly BooksController _sut;

        public BooksControllerMoreTests()
        {
            _sut = new BooksController(_bookService.Object, _logger.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // ensure HttpContext.TraceIdentifier is present (controller uses it)
            _sut.HttpContext.TraceIdentifier = "test-trace-id";
        }

        [Fact]
        public async Task GetBooks_Forwards_Filters_To_Service()
        {
            // Arrange
            var genre = "Fantasy";
            int? year = 2023;
            double? minRating = 4.5;

            var all = new List<BookDto>
            {
                new() { Id = 1, Title = "F1", Author = "A1" },
                new() { Id = 2, Title = "F2", Author = "A2" },
            };

            _bookService
                .Setup(s => s.GetAllBooksAsync(genre, year, minRating))
                .ReturnsAsync(all);

            // Act
            var action = await _sut.GetBooks(genre, year, minRating, page: 1, pageSize: 10);

            // Assert
            action.Result.Should().BeOfType<OkObjectResult>();
            _bookService.Verify(s => s.GetAllBooksAsync(genre, year, minRating), Times.Once);
        }

        [Fact]
        public async Task DeleteBook_WhenExists_Returns204()
        {
            _bookService.Setup(s => s.DeleteBookAsync(7)).ReturnsAsync(true);

            var result = await _sut.DeleteBook(7);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task GetBooks_WhenException_LogsAndReturns500()
        {
            _bookService.Setup(s => s.GetAllBooksAsync(null, null, null))
                        .ThrowsAsync(new Exception("kaboom"));

            var action = await _sut.GetBooks();

            // Assert 500 with ApiResponse<object>
            action.Result.Should().BeOfType<ObjectResult>();
            var obj = (ObjectResult)action.Result!;
            obj.StatusCode.Should().Be(500);
            obj.Value.Should().BeOfType<ApiResponse<object>>();

            // Verify we logged error at least once
            _logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error occurred while fetching books")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
