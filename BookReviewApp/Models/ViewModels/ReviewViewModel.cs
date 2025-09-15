using BookReviewApp.Common;
using System.ComponentModel.DataAnnotations;

namespace BookReviewApp.Models.ViewModels
{
    public class ReviewViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ValidationMessages.ReviewContentRequired)]
        [Display(Name = "Review")]
        [StringLength(2000, MinimumLength = 10,
            ErrorMessage = ValidationMessages.ReviewRange)]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessages.RatingRequired)]
        [Display(Name = "Rating")]
        [Range(1, 5, ErrorMessage = ValidationMessages.RatingRange)]
        public int Rating { get; set; }

        [Display(Name = "Date")]
        public DateTime DateCreated { get; set; }

        public int BookId { get; set; }

        [Display(Name = "Book")]
        public string BookTitle { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        [Display(Name = "User")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Upvotes")]
        public int UpvoteCount { get; set; }

        [Display(Name = "Downvotes")]
        public int DownvoteCount { get; set; }

        [Display(Name = "Net Votes")]
        public int NetVotes => UpvoteCount - DownvoteCount;

        public bool? UserVote { get; set; } // null = no vote, true = upvote, false = downvote
    }
}