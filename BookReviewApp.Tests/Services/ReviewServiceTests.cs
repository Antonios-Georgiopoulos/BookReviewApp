using BookReviewApp.Services;
using BookReviewApp.Data.Repositories.Interfaces;

namespace BookReviewApp.Tests.Services
{
    public class ReviewServiceTests : IDisposable
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly ReviewService _reviewService;

        public ReviewServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _reviewService = new ReviewService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task ReviewExistsAsync_ShouldReturnTrue_WhenReviewExists()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.Reviews.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Review, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _reviewService.ReviewExistsAsync(1);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UserHasReviewedBookAsync_ShouldReturnTrue_WhenUserHasReviewed()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.Reviews.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Review, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _reviewService.UserHasReviewedBookAsync(1, "user1");

            // Assert
            result.Should().BeTrue();
        }

        public void Dispose()
        {
            // Cleanup if needed
        }
    }
}