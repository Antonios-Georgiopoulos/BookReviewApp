using BookReviewApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BookReviewApp.Infrastructure.HealthChecks
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly ApplicationDbContext _context;

        public DatabaseHealthCheck(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.Database.CanConnectAsync(cancellationToken);

                var bookCount = await _context.Books.CountAsync(cancellationToken);
                var userCount = await _context.Users.CountAsync(cancellationToken);

                var data = new Dictionary<string, object>
                {
                    { "books_count", bookCount },
                    { "users_count", userCount },
                    { "database_provider", _context.Database.ProviderName ?? "Unknown" }
                };

                return HealthCheckResult.Healthy("Database is healthy", data);
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Database is unhealthy", ex);
            }
        }
    }
}