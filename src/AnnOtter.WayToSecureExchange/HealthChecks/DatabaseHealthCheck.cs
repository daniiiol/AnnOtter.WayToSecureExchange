using AnnOtter.WayToSecureExchange.Databases;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AnnOtter.WayToSecureExchange.HealthChecks
{
    /// <summary>
    /// Health check that verifies the application can connect to the database.
    /// Returns <see cref="HealthCheckResult.Unhealthy"/> if the connection fails.
    /// </summary>
    public class DatabaseHealthCheck(IServiceScopeFactory scopeFactory, ILogger<DatabaseHealthCheck> logger) : IHealthCheck
    {
        /// <summary>
        /// Attempts to connect to the database using a scoped <see cref="ExchangeDatabaseContext"/>.
        /// </summary>
        /// <param name="context">Provides access to the health check registration and its configuration.</param>
        /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
        /// <returns>
        /// <see cref="HealthCheckResult.Healthy"/> if the database is reachable;
        /// <see cref="HealthCheckResult.Unhealthy"/> otherwise.
        /// </returns>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ExchangeDatabaseContext>();
                var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

                if (!canConnect)
                {
                    return HealthCheckResult.Unhealthy(description: "Database is not reachable.");
                }

                return HealthCheckResult.Healthy(description: "Database is reachable.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database health check failed.");

                return HealthCheckResult.Unhealthy(description: "Database check failed. See server logs for details.");
            }
        }
    }
}
