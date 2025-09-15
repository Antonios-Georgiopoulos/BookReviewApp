using BookReviewApp.Services;

namespace BookReviewApp.Tests.Services
{
    public class ReviewServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ReviewService _reviewService;

        public ReviewServiceTests()
        {
            _context = TestDbContext.CreateInMemoryContext();
            TestDbContext.SeedTestData(_context);
            _reviewService = new ReviewService(_context);
        }

        [Fact]
        public async Task GetReviewsByBookIdAsync_ShouldReturnReviews_ForExistingBook()
        {
            // Act
            var result = await _reviewService.GetReviewsByBookIdAsync(1);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(r => r.Content == "Great book!");
            result.Should().Contain(r => r.Content == "Good read");
        }

        [Fact]
        public async Task GetReviewByIdAsync_ShouldReturnReview_WhenReviewExists()
        {
            // Act
            var result = await _reviewService.GetReviewByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Content.Should().Be("Great book!");
            result.Rating.Should().Be(5);
            result.UserId.Should().Be("user1");
        }

        [Fact]
        public async Task CreateReviewAsync_ShouldCreateReview_WithValidData()
        {
            // Arrange
            var createReviewDto = new CreateReviewDto
            {
                Content = "Amazing book!",
                Rating = 5,
                BookId = 2
            };

            // Act
            var result = await _reviewService.CreateReviewAsync(createReviewDto, "user1");

            // Assert
            result.Should().NotBeNull();
            result.Content.Should().Be("Amazing book!");
            result.Rating.Should().Be(5);
            result.BookId.Should().Be(2);
            result.UserId.Should().Be("user1");
        }

        [Fact]
        public async Task UserHasReviewedBookAsync_ShouldReturnTrue_WhenUserHasReviewed()
        {
            // Act
            var result = await _reviewService.UserHasReviewedBookAsync(1, "user1");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ReviewExistsAsync_ShouldReturnTrue_WhenReviewExists()
        {
            // Act
            var result = await _reviewService.ReviewExistsAsync(1);

            // Assert
            result.Should().BeTrue();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}