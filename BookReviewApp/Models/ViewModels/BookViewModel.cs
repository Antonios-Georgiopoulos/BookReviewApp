using System.ComponentModel.DataAnnotations;

namespace BookReviewApp.Models.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ο τίτλος είναι υποχρεωτικός")]
        [Display(Name = "Τίτλος")]
        [StringLength(200, ErrorMessage = "Ο τίτλος δεν μπορεί να υπερβαίνει τους 200 χαρακτήρες")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ο συγγραφέας είναι υποχρεωτικός")]
        [Display(Name = "Συγγραφέας")]
        [StringLength(100, ErrorMessage = "Το όνομα του συγγραφέα δεν μπορεί να υπερβαίνει τους 100 χαρακτήρες")]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = "Το έτος έκδοσης είναι υποχρεωτικό")]
        [Display(Name = "Έτος Έκδοσης")]
        [Range(1000, 2100, ErrorMessage = "Το έτος πρέπει να είναι μεταξύ 1000 και 2100")]
        public int PublishedYear { get; set; }

        [Required(ErrorMessage = "Το είδος είναι υποχρεωτικό")]
        [Display(Name = "Είδος")]
        [StringLength(50, ErrorMessage = "Το είδος δεν μπορεί να υπερβαίνει τους 50 χαρακτήρες")]
        public string Genre { get; set; } = string.Empty;

        [Display(Name = "Ημερομηνία Δημιουργίας")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Μέση Αξιολόγηση")]
        public double AverageRating { get; set; }

        [Display(Name = "Αριθμός Κριτικών")]
        public int ReviewCount { get; set; }
    }
}