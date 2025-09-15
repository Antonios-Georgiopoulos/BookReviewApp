using BookReviewApp.Models.Domain;

namespace BookReviewApp.Data.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Book> Books { get; }
        IGenericRepository<Review> Reviews { get; }
        IGenericRepository<ReviewVote> ReviewVotes { get; }
        IGenericRepository<User> Users { get; }

        Task<int> SaveAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}