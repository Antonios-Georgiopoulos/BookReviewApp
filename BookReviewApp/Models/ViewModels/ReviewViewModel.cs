using BookReviewApp.Common;
using System.ComponentModel.DataAnnotations;

namespace BookReviewApp.Models.ViewModels
{
    public class ReviewViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ValidationMessages.ReviewContentRequired)]
        [Display(Name = "Κριτική")]
        [StringLength(2000, MinimumLength = 10,
            ErrorMessage = ValidationMessages.ReviewRange)]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessages.RatingRequired)]
        [Display(Name = "Αξιολόγηση")]
        [Range(1, 5, ErrorMessage = ValidationMessages.RatingRange)]
        public int Rating { get; set; }

        [Display(Name = "Ημερομηνία")]
        public DateTime DateCreated { get; set; }

        public int BookId { get; set; }

        [Display(Name = "Βιβλίο")]
        public string BookTitle { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        [Display(Name = "Χρήστης")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Θετικές Ψήφοι")]
        public int UpvoteCount { get; set; }

        [Display(Name = "Αρνητικές Ψήφοι")]
        public int DownvoteCount { get; set; }

        [Display(Name = "Συνολικές Ψήφοι")]
        public int NetVotes => UpvoteCount - DownvoteCount;

        public bool? UserVote { get; set; } // null = no vote, true = upvote, false = downvote
    }
}