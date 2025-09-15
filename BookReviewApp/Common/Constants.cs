namespace BookReviewApp.Common
{
    public static class CacheKeys
    {
        public const string AllBooks = "books_all_{0}_{1}_{2}"; // genre, year, minRating
        public const string BookById = "book_{0}";
        public const string BookDetails = "book_details_{0}_{1}"; // bookId, userId
        public const string BookGenres = "book_genres";
        public const string BookYears = "book_years";
        public const string ReviewsByBook = "reviews_book_{0}";
        public const string ReviewById = "review_{0}";

        public static string GetBookCacheKey(int bookId) => string.Format(BookById, bookId);
        public static string GetBooksCacheKey(string? genre, int? year, double? minRating) =>
            string.Format(AllBooks, genre ?? "null", year?.ToString() ?? "null", minRating?.ToString() ?? "null");
        public static string GetReviewsCacheKey(int bookId) => string.Format(ReviewsByBook, bookId);
    }

    public static class ActionNames
    {
        public const string Index = nameof(Index);
        public const string Details = nameof(Details);
        public const string Create = nameof(Create);
        public const string Edit = nameof(Edit);
        public const string Delete = nameof(Delete);
        public const string AddReview = nameof(AddReview);
        public const string VoteReview = nameof(VoteReview);
    }

    public static class ControllerNames
    {
        public const string Books = "Books";
        public const string Reviews = "Reviews";
        public const string Home = "Home";
    }

    public static class ErrorMessages
    {
        public const string BookNotFound = "Book not found.";
        public const string ReviewNotFound = "Review not found.";
        public const string UserNotAuthenticated = "User not authenticated.";
        public const string BookAlreadyReviewed = "User has already reviewed this book.";
        public const string CannotVoteOwnReview = "Users cannot vote on their own reviews.";
        public const string BookDoesNotExist = "Book does not exist.";
        public const string UnauthorizedReviewAccess = "User can only modify their own reviews.";
        public const string GenericError = "An error occurred while processing your request.";
    }

    public static class SuccessMessages
    {
        public const string BookCreated = "Book created successfully!";
        public const string BookUpdated = "Book updated successfully!";
        public const string BookDeleted = "Book deleted successfully!";
        public const string ReviewAdded = "Review added successfully!";
        public const string ReviewUpdated = "Review updated successfully!";
        public const string ReviewDeleted = "Review deleted successfully!";
        public const string VoteRecorded = "Vote recorded successfully!";
    }

    public static class TempDataKeys
    {
        public const string Success = "Success";
        public const string Error = "Error";
    }

    public static class ValidationMessages
    {
        public const string TitleRequired = "Title is required";
        public const string AuthorRequired = "Author is required";
        public const string GenreRequired = "Genre is required";
        public const string YearRequired = "Publication year is required";
        public const string ReviewContentRequired = "Review content is required";
        public const string RatingRequired = "Rating is required";
        public const string RatingRange = "Rating must be between 1 and 5";
        public const string ReviewRange = "Review must be between 10 and 2000 characters";
        public const string TitleRange = "Title cannot exceed 200 characters";
        public const string AuthorRange = "Author name cannot exceed 100 characters";
        public const string GenreRange = "Genre cannot exceed 50 characters";
        public const string YearRange = "Year must be between 1000 and 2100";
    }
}