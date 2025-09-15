namespace BookReviewApp.Common
{
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
        public const string BookCreated = "Το βιβλίο δημιουργήθηκε επιτυχώς!";
        public const string BookUpdated = "Το βιβλίο ενημερώθηκε επιτυχώς!";
        public const string BookDeleted = "Το βιβλίο διαγράφηκε επιτυχώς!";
        public const string ReviewAdded = "Η κριτική σας προστέθηκε επιτυχώς!";
        public const string ReviewUpdated = "Η κριτική ενημερώθηκε επιτυχώς!";
        public const string ReviewDeleted = "Η κριτική διαγράφηκε επιτυχώς!";
        public const string VoteRecorded = "Η ψήφος καταγράφηκε επιτυχώς!";
    }

    public static class TempDataKeys
    {
        public const string Success = "Success";
        public const string Error = "Error";
    }

    public static class ValidationMessages
    {
        public const string TitleRequired = "Ο τίτλος είναι υποχρεωτικός";
        public const string AuthorRequired = "Ο συγγραφέας είναι υποχρεωτικός";
        public const string GenreRequired = "Το είδος είναι υποχρεωτικό";
        public const string YearRequired = "Το έτος έκδοσης είναι υποχρεωτικό";
        public const string ReviewContentRequired = "Η κριτική είναι υποχρεωτική";
        public const string RatingRequired = "Η αξιολόγηση είναι υποχρεωτική";
        public const string RatingRange = "Η αξιολόγηση πρέπει να είναι μεταξύ 1 και 5";
        public const string ReviewRange = "Η κριτική πρέπει να είναι μεταξύ 10 και 2000 χαρακτήρων";
        public const string TitleRange = "Ο τίτλος δεν μπορεί να υπερβαίνει τους 200 χαρακτήρες";
        public const string AuthorRange = "Το όνομα του συγγραφέα δεν μπορεί να υπερβαίνει τους 100 χαρακτήρες";
        public const string GenreRange = "Το είδος δεν μπορεί να υπερβαίνει τους 50 χαρακτήρες";
        public const string YearRange = "Το έτος πρέπει να είναι μεταξύ 1000 και 2100";
    }
}