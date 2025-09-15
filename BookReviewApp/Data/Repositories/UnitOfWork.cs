using BookReviewApp.Data.Repositories.Interfaces;
using BookReviewApp.Models.Domain;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookReviewApp.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        private IGenericRepository<Book>? _books;
        private IGenericRepository<Review>? _reviews;
        private IGenericRepository<ReviewVote>? _reviewVotes;
        private IGenericRepository<User>? _users;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<Book> Books
        {
            get { return _books ??= new GenericRepository<Book>(_context); }
        }

        public IGenericRepository<Review> Reviews
        {
            get { return _reviews ??= new GenericRepository<Review>(_context); }
        }

        public IGenericRepository<ReviewVote> ReviewVotes
        {
            get { return _reviewVotes ??= new GenericRepository<ReviewVote>(_context); }
        }

        public IGenericRepository<User> Users
        {
            get { return _users ??= new GenericRepository<User>(_context); }
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}