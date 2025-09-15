using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookReviewApp.Models.ViewModels
{
    public class BookListViewModel
    {
        public IEnumerable<BookViewModel> Books { get; set; } = [];

        // Filters
        public string? SearchTerm { get; set; }
        public string? SelectedGenre { get; set; }
        public int? PublishedYear { get; set; }
        public double? MinRating { get; set; }

        // Sorting
        public string SortBy { get; set; } = "Title";
        public string SortOrder { get; set; } = "asc";

        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalBooks { get; set; }

        // Dropdown lists for filters
        public IEnumerable<SelectListItem> GenreOptions { get; set; } = [];
        public IEnumerable<SelectListItem> YearOptions { get; set; } = [];
        public IEnumerable<SelectListItem> SortOptions { get; set; } =
        [
            new() { Value = "Title", Text = "Title" },
            new() { Value = "Author", Text = "Author" },
            new() { Value = "PublishedYear", Text = "Publication Year" },
            new() { Value = "AverageRating", Text = "Rating" },
            new() { Value = "DateCreated", Text = "Date Added" }
        ];
    }
}