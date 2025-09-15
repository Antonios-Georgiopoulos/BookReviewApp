namespace BookReviewApp.Models.Api
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? TraceId { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Operation completed successfully")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }

    public class PaginatedResponse<T> : ApiResponse<IEnumerable<T>>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasNextPage => CurrentPage < TotalPages;
        public bool HasPreviousPage => CurrentPage > 1;

        public static PaginatedResponse<T> CreateResponse(
            IEnumerable<T> data,
            int currentPage,
            int pageSize,
            int totalCount,
            string message = "Data retrieved successfully")
        {
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PaginatedResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalCount = totalCount
            };
        }
    }
}