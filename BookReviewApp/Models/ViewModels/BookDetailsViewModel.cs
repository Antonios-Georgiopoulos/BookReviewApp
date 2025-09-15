namespace BookReviewApp.Models.ViewModels
{
    public class BookDetailsViewModel
    {
        public BookViewModel Book { get; set; } = new BookViewModel();
        public IEnumerable<ReviewViewModel> Reviews { get; set; } = [];
        public ReviewViewModel NewReview { get; set; } = new ReviewViewModel();
        public bool CanAddReview { get; set; }
        public bool UserHasReviewed { get; set; }
    }
}