using System.ComponentModel.DataAnnotations;

namespace BookReviewApp.Models.Domain
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Rating { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public int BookId { get; set; }
        public string UserId { get; set; } = string.Empty;

        // Navigation properties
        public virtual Book Book { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<ReviewVote> ReviewVotes { get; set; } = [];
    }
}