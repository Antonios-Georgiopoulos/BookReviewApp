using BookReviewApp.Data.Repositories.Interfaces;
using BookReviewApp.Models.Domain;
using BookReviewApp.Models.ViewModels.Api;
using BookReviewApp.Services.Interfaces;

namespace BookReviewApp.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByBookIdAsync(int bookId)
        {
            var reviews = await _unitOfWork.Reviews.GetAllAsync(
                filter: r => r.BookId == bookId,
                orderBy: reviews => reviews.OrderByDescending(r => r.DateCreated),
                includeProperties: "User,Book,ReviewVotes"
            );

            return reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                Content = r.Content,
                Rating = r.Rating,
                DateCreated = r.DateCreated,
                BookId = r.BookId,
                BookTitle = r.Book.Title,
                UserId = r.UserId,
                UserName = r.User.UserName ?? "Unknown",
                UpvoteCount = r.ReviewVotes.Count(rv => rv.IsUpvote),
                DownvoteCount = r.ReviewVotes.Count(rv => !rv.IsUpvote),
                NetVotes = r.ReviewVotes.Count(rv => rv.IsUpvote) - r.ReviewVotes.Count(rv => !rv.IsUpvote)
            });
        }

        public async Task<ReviewDto?> GetReviewByIdAsync(int id)
        {
            var review = await _unitOfWork.Reviews.GetFirstOrDefaultAsync(
                filter: r => r.Id == id,
                includeProperties: "User,Book,ReviewVotes"
            );

            if (review == null) return null;

            return new ReviewDto
            {
                Id = review.Id,
                Content = review.Content,
                Rating = review.Rating,
                DateCreated = review.DateCreated,
                BookId = review.BookId,
                BookTitle = review.Book.Title,
                UserId = review.UserId,
                UserName = review.User.UserName ?? "Unknown",
                UpvoteCount = review.ReviewVotes.Count(rv => rv.IsUpvote),
                DownvoteCount = review.ReviewVotes.Count(rv => !rv.IsUpvote),
                NetVotes = review.ReviewVotes.Count(rv => rv.IsUpvote) - review.ReviewVotes.Count(rv => !rv.IsUpvote)
            };
        }

        public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto createReviewDto, string userId)
        {
            var existingReview = await _unitOfWork.Reviews.GetFirstOrDefaultAsync(
                filter: r => r.BookId == createReviewDto.BookId && r.UserId == userId
            );

            if (existingReview != null)
            {
                throw new InvalidOperationException("User has already reviewed this book.");
            }

            var bookExists = await _unitOfWork.Books.ExistsAsync(b => b.Id == createReviewDto.BookId);
            if (!bookExists)
            {
                throw new ArgumentException("Book does not exist.");
            }

            var review = new Review
            {
                Content = createReviewDto.Content,
                Rating = createReviewDto.Rating,
                BookId = createReviewDto.BookId,
                UserId = userId,
                DateCreated = DateTime.UtcNow
            };

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveAsync();

            var createdReview = await _unitOfWork.Reviews.GetFirstOrDefaultAsync(
                filter: r => r.Id == review.Id,
                includeProperties: "User,Book,ReviewVotes"
            );

            return new ReviewDto
            {
                Id = createdReview!.Id,
                Content = createdReview.Content,
                Rating = createdReview.Rating,
                DateCreated = createdReview.DateCreated,
                BookId = createdReview.BookId,
                BookTitle = createdReview.Book.Title,
                UserId = createdReview.UserId,
                UserName = createdReview.User.UserName ?? "Unknown",
                UpvoteCount = 0,
                DownvoteCount = 0,
                NetVotes = 0
            };
        }

        public async Task<ReviewDto?> UpdateReviewAsync(int id, CreateReviewDto updateReviewDto, string userId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review == null) return null;

            if (review.UserId != userId)
            {
                throw new UnauthorizedAccessException("User can only update their own reviews.");
            }

            review.Content = updateReviewDto.Content;
            review.Rating = updateReviewDto.Rating;

            _unitOfWork.Reviews.Update(review);
            await _unitOfWork.SaveAsync();

            return await GetReviewByIdAsync(id);
        }

        public async Task<bool> DeleteReviewAsync(int id, string userId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review == null) return false;

            if (review.UserId != userId)
            {
                throw new UnauthorizedAccessException("User can only delete their own reviews.");
            }

            _unitOfWork.Reviews.Delete(review);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> UserHasReviewedBookAsync(int bookId, string userId)
        {
            return await _unitOfWork.Reviews.ExistsAsync(r => r.BookId == bookId && r.UserId == userId);
        }

        public async Task<bool> ReviewExistsAsync(int id)
        {
            return await _unitOfWork.Reviews.ExistsAsync(r => r.Id == id);
        }

        public async Task<bool> VoteOnReviewAsync(int reviewId, string userId, bool isUpvote)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review == null) return false;

            if (review.UserId == userId)
            {
                throw new InvalidOperationException("Users cannot vote on their own reviews.");
            }

            var existingVote = await _unitOfWork.ReviewVotes.GetFirstOrDefaultAsync(
                filter: rv => rv.ReviewId == reviewId && rv.UserId == userId
            );

            if (existingVote != null)
            {
                if (existingVote.IsUpvote == isUpvote)
                {
                    _unitOfWork.ReviewVotes.Delete(existingVote);
                }
                else
                {
                    existingVote.IsUpvote = isUpvote;
                    existingVote.DateCreated = DateTime.UtcNow;
                    _unitOfWork.ReviewVotes.Update(existingVote);
                }
            }
            else
            {
                var newVote = new ReviewVote
                {
                    ReviewId = reviewId,
                    UserId = userId,
                    IsUpvote = isUpvote,
                    DateCreated = DateTime.UtcNow
                };
                await _unitOfWork.ReviewVotes.AddAsync(newVote);
            }

            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool?> GetUserVoteOnReviewAsync(int reviewId, string userId)
        {
            var vote = await _unitOfWork.ReviewVotes.GetFirstOrDefaultAsync(
                filter: rv => rv.ReviewId == reviewId && rv.UserId == userId
            );

            return vote?.IsUpvote;
        }
    }
}