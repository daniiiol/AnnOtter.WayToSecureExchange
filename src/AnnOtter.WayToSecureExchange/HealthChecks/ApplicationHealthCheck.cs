using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AnnOtter.WayToSecureExchange.HealthChecks
{
    /// <summary>
    /// Health check that monitors the general application health based on managed memory usage.
    /// Returns <see cref="HealthCheckResult.Degraded"/> if memory usage exceeds the configured threshold.
    /// </summary>
    public class ApplicationHealthCheck : IHealthCheck
    {
        private static readonly long MemoryThresholdBytes = 1024L * 1024 * 1024; // 1024 MB

        /// <summary>
        /// Evaluates the current memory allocation of the process against the configured threshold.
        /// </summary>
        /// <param name="context">Provides access to the health check registration and its configuration.</param>
        /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
        /// <returns>
        /// <see cref="HealthCheckResult.Healthy"/> if memory usage is within the threshold;
        /// <see cref="HealthCheckResult.Degraded"/> with a description containing the current memory usage in MB and uptime in seconds if the threshold is exceeded.
        /// </returns>
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var allocatedBytes = GC.GetTotalMemory(forceFullCollection: false);
            var uptime = Environment.TickCount64;

            if (allocatedBytes > MemoryThresholdBytes)
            {
                return Task.FromResult(HealthCheckResult.Degraded(
                    description: $"Memory usage is high: {Math.Round(allocatedBytes / 1024.0 / 1024.0, 2)} MB allocated, uptime: {uptime / 1000}s."));
            }

            return Task.FromResult(HealthCheckResult.Healthy(description: "Application is running within normal parameters."));
        }
    }
}
