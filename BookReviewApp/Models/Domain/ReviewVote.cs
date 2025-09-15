namespace BookReviewApp.Models.Domain
{
    public class ReviewVote
    {
        public int Id { get; set; }

        public bool IsUpvote { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public int ReviewId { get; set; }
        public string UserId { get; set; } = string.Empty;

        // Navigation properties
        public virtual Review Review { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}