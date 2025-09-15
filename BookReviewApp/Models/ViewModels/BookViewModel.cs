using BookReviewApp.Common;
using System.ComponentModel.DataAnnotations;

namespace BookReviewApp.Models.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ValidationMessages.TitleRequired)]
        [Display(Name = "Τίτλος")]
        [StringLength(200, ErrorMessage = ValidationMessages.TitleRange)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessages.AuthorRequired)]
        [Display(Name = "Συγγραφέας")]
        [StringLength(100, ErrorMessage = ValidationMessages.AuthorRange)]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessages.YearRequired)]
        [Display(Name = "Έτος Έκδοσης")]
        [Range(1000, 2100, ErrorMessage = ValidationMessages.YearRange)]
        public int PublishedYear { get; set; }

        [Required(ErrorMessage = ValidationMessages.GenreRequired)]
        [Display(Name = "Είδος")]
        [StringLength(50, ErrorMessage = ValidationMessages.GenreRange)]
        public string Genre { get; set; } = string.Empty;

        [Display(Name = "Ημερομηνία Δημιουργίας")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Μέση Αξιολόγηση")]
        public double AverageRating { get; set; }

        [Display(Name = "Αριθμός Κριτικών")]
        public int ReviewCount { get; set; }
    }
}