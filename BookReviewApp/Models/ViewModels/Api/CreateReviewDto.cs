using System.ComponentModel.DataAnnotations;

namespace BookReviewApp.Models.ViewModels.Api
{
    public class CreateReviewDto
    {
        [Required]
        [StringLength(2000, MinimumLength = 10)]
        public string Content { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Rating { get; set; }

        public int BookId { get; set; }
    }
}