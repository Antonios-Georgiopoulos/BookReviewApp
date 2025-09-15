using Microsoft.Extensions.DependencyInjection;

namespace BookReviewApp.Tests
{
    public class TestDbContext
    {
        public static ApplicationDbContext CreateInMemoryContext()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            return new ApplicationDbContext(options);
        }

        public static ApplicationDbContext SeedTestData(ApplicationDbContext context)
        {
            var users = new List<User>
            {
                new() { Id = "user1", UserName = "testuser1", Email = "test1@test.com" },
                new() { Id = "user2", UserName = "testuser2", Email = "test2@test.com" }
            };

            var books = new List<Book>
            {
                new() {
                    Id = 1,
                    Title = "Test Book 1",
                    Author = "Test Author 1",
                    Genre = "Fiction",
                    PublishedYear = 2023,
                    DateCreated = DateTime.UtcNow
                },
                new() {
                    Id = 2,
                    Title = "Test Book 2",
                    Author = "Test Author 2",
                    Genre = "Non-Fiction",
                    PublishedYear = 2022,
                    DateCreated = DateTime.UtcNow
                }
            };

            var reviews = new List<Review>
            {
                new() {
                    Id = 1,
                    Content = "Great book!",
                    Rating = 5,
                    BookId = 1,
                    UserId = "user1",
                    DateCreated = DateTime.UtcNow
                },
                new() {
                    Id = 2,
                    Content = "Good read",
                    Rating = 4,
                    BookId = 1,
                    UserId = "user2",
                    DateCreated = DateTime.UtcNow
                }
            };

            context.Users.AddRange(users);
            context.Books.AddRange(books);
            context.Reviews.AddRange(reviews);
            context.SaveChanges();

            return context;
        }
    }
}