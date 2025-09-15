using BookReviewApp.Models.ViewModels.Api;

namespace BookReviewApp.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetReviewsByBookIdAsync(int bookId);
        Task<ReviewDto?> GetReviewByIdAsync(int id);
        Task<ReviewDto> CreateReviewAsync(CreateReviewDto createReviewDto, string userId);
        Task<ReviewDto?> UpdateReviewAsync(int id, CreateReviewDto updateReviewDto, string userId);
        Task<bool> DeleteReviewAsync(int id, string userId);
        Task<bool> UserHasReviewedBookAsync(int bookId, string userId);
        Task<bool> ReviewExistsAsync(int id);
        Task<bool> VoteOnReviewAsync(int reviewId, string userId, bool isUpvote);
        Task<bool?> GetUserVoteOnReviewAsync(int reviewId, string userId);
    }
}