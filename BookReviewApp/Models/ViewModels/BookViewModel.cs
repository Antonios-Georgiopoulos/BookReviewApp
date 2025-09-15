using BookReviewApp.Common;
using System.ComponentModel.DataAnnotations;

namespace BookReviewApp.Models.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ValidationMessages.TitleRequired)]
        [Display(Name = "Title")]
        [StringLength(200, ErrorMessage = ValidationMessages.TitleRange)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessages.AuthorRequired)]
        [Display(Name = "Author")]
        [StringLength(100, ErrorMessage = ValidationMessages.AuthorRange)]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessages.YearRequired)]
        [Display(Name = "Publication Year")]
        [Range(1000, 2100, ErrorMessage = ValidationMessages.YearRange)]
        public int PublishedYear { get; set; }

        [Required(ErrorMessage = ValidationMessages.GenreRequired)]
        [Display(Name = "Genre")]
        [StringLength(50, ErrorMessage = ValidationMessages.GenreRange)]
        public string Genre { get; set; } = string.Empty;

        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Average Rating")]
        public double AverageRating { get; set; }

        [Display(Name = "Review Count")]
        public int ReviewCount { get; set; }
    }
}