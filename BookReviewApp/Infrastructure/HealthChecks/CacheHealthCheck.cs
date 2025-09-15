using BookReviewApp.Services.Caching;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BookReviewApp.Infrastructure.HealthChecks
{
    public class CacheHealthCheck : IHealthCheck
    {
        private readonly ICacheService _cacheService;

        public CacheHealthCheck(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var testKey = $"health_check_{Guid.NewGuid()}";
                var testValue = "test";

                await _cacheService.SetAsync(testKey, testValue, TimeSpan.FromSeconds(10));
                var cachedValue = await _cacheService.GetAsync<string>(testKey);
                await _cacheService.RemoveAsync(testKey);

                if (cachedValue == testValue)
                {
                    return HealthCheckResult.Healthy("Cache is working properly");
                }

                return HealthCheckResult.Degraded("Cache is not working as expected");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Cache is unhealthy", ex);
            }
        }
    }
}