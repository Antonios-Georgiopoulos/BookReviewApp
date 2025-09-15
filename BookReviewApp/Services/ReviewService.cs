using BookReviewApp.Data;
using BookReviewApp.Models.Domain;
using BookReviewApp.Models.ViewModels.Api;
using BookReviewApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByBookIdAsync(int bookId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Book)
                .Include(r => r.ReviewVotes)
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.DateCreated)
                .ToListAsync();

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
            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Book)
                .Include(r => r.ReviewVotes)
                .FirstOrDefaultAsync(r => r.Id == id);

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
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.BookId == createReviewDto.BookId && r.UserId == userId);

            if (existingReview != null)
            {
                throw new InvalidOperationException("User has already reviewed this book.");
            }

            var bookExists = await _context.Books.AnyAsync(b => b.Id == createReviewDto.BookId);
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

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var createdReview = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Book)
                .Include(r => r.ReviewVotes)
                .FirstAsync(r => r.Id == review.Id);

            return new ReviewDto
            {
                Id = createdReview.Id,
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
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return null;

            if (review.UserId != userId)
            {
                throw new UnauthorizedAccessException("User can only update their own reviews.");
            }

            review.Content = updateReviewDto.Content;
            review.Rating = updateReviewDto.Rating;

            await _context.SaveChangesAsync();

            return await GetReviewByIdAsync(id);
        }

        public async Task<bool> DeleteReviewAsync(int id, string userId)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return false;

            if (review.UserId != userId)
            {
                throw new UnauthorizedAccessException("User can only delete their own reviews.");
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserHasReviewedBookAsync(int bookId, string userId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.BookId == bookId && r.UserId == userId);
        }

        public async Task<bool> ReviewExistsAsync(int id)
        {
            return await _context.Reviews.AnyAsync(r => r.Id == id);
        }

        public async Task<bool> VoteOnReviewAsync(int reviewId, string userId, bool isUpvote)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return false;

            if (review.UserId == userId)
            {
                throw new InvalidOperationException("Users cannot vote on their own reviews.");
            }

            var existingVote = await _context.ReviewVotes
                .FirstOrDefaultAsync(rv => rv.ReviewId == reviewId && rv.UserId == userId);

            if (existingVote != null)
            {
                if (existingVote.IsUpvote == isUpvote)
                {
                    _context.ReviewVotes.Remove(existingVote);
                }
                else
                {
                    existingVote.IsUpvote = isUpvote;
                    existingVote.DateCreated = DateTime.UtcNow;
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
                _context.ReviewVotes.Add(newVote);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool?> GetUserVoteOnReviewAsync(int reviewId, string userId)
        {
            var vote = await _context.ReviewVotes
                .FirstOrDefaultAsync(rv => rv.ReviewId == reviewId && rv.UserId == userId);

            return vote?.IsUpvote;
        }
    }
}