using BookReviewApp.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            // Check if database already has data
            if (context.Books.Any())
            {
                return; // Database has been seeded
            }

            // Create ONLY books - no users
            var books = CreateBooks();
            context.Books.AddRange(books);
            await context.SaveChangesAsync();

            // No reviews created since there are no users
            // Reviews will be created by users who register
        }

        private static List<Book> CreateBooks()
        {
            return
            [
                new() {
                    Title = "The Name of the Rose",
                    Author = "Umberto Eco",
                    PublishedYear = 1980,
                    Genre = "Mystery",
                    DateCreated = DateTime.UtcNow.AddDays(-30)
                },
                new() {
                    Title = "1984",
                    Author = "George Orwell",
                    PublishedYear = 1949,
                    Genre = "Science Fiction",
                    DateCreated = DateTime.UtcNow.AddDays(-25)
                },
                new() {
                    Title = "One Hundred Years of Solitude",
                    Author = "Gabriel García Márquez",
                    PublishedYear = 1967,
                    Genre = "Fiction",
                    DateCreated = DateTime.UtcNow.AddDays(-20)
                },
                new() {
                    Title = "The Lord of the Rings",
                    Author = "J.R.R. Tolkien",
                    PublishedYear = 1954,
                    Genre = "Fantasy",
                    DateCreated = DateTime.UtcNow.AddDays(-15)
                },
                new() {
                    Title = "To Kill a Mockingbird",
                    Author = "Harper Lee",
                    PublishedYear = 1960,
                    Genre = "Fiction",
                    DateCreated = DateTime.UtcNow.AddDays(-12)
                },
                new() {
                    Title = "The Little Prince",
                    Author = "Antoine de Saint-Exupéry",
                    PublishedYear = 1943,
                    Genre = "Children",
                    DateCreated = DateTime.UtcNow.AddDays(-10)
                },
                new() {
                    Title = "Sapiens: A Brief History of Humankind",
                    Author = "Yuval Noah Harari",
                    PublishedYear = 2011,
                    Genre = "History",
                    DateCreated = DateTime.UtcNow.AddDays(-5)
                },
                new() {
                    Title = "The Art of Happiness",
                    Author = "Dalai Lama",
                    PublishedYear = 1998,
                    Genre = "Philosophy",
                    DateCreated = DateTime.UtcNow.AddDays(-3)
                },
                new() {
                    Title = "Dune",
                    Author = "Frank Herbert",
                    PublishedYear = 1965,
                    Genre = "Science Fiction",
                    DateCreated = DateTime.UtcNow.AddDays(-2)
                },
                new() {
                    Title = "Pride and Prejudice",
                    Author = "Jane Austen",
                    PublishedYear = 1813,
                    Genre = "Romance",
                    DateCreated = DateTime.UtcNow.AddDays(-1)
                }
            ];
        }
    }
}