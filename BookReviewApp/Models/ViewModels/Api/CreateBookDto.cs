using System.ComponentModel.DataAnnotations;

namespace BookReviewApp.Models.ViewModels.Api
{
    public class CreateBookDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Author { get; set; } = string.Empty;

        [Range(1000, 2100)]
        public int PublishedYear { get; set; }

        [Required]
        [StringLength(50)]
        public string Genre { get; set; } = string.Empty;
    }
}