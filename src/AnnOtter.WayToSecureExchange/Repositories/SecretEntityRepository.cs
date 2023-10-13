using AnnOtter.WayToSecureExchange.Configuration.AppSettings;
using AnnOtter.WayToSecureExchange.Controllers.API;
using AnnOtter.WayToSecureExchange.Databases;
using AnnOtter.WayToSecureExchange.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AnnOtter.WayToSecureExchange.Repositories
{
    /// <summary>
    /// Repository class for managing the SecretEntity.
    /// </summary>
    public class SecretEntityRepository : ISecretEntityRepository
    {
        private readonly ILogger _logger;
        private readonly IOptions<MainSettings> _config;
        private readonly ExchangeDatabaseContext _dbContext;

        /// <summary>
        /// Ctor of SecretEntity Repository with dependency-injected ISecretEntityRepository implementation
        /// </summary>
        /// <param name="logger">The DI implementation of ILogger should be used normally. Here, you can overrite it.</param>
        /// <param name="configuration">The DI implementation of IOptions\<MainSettings\> should be used normally. Here, you can overrite it.</param>
        public SecretEntityRepository(ILogger<SecretEntityRepository> logger, IOptions<MainSettings> configuration)
        {
            _dbContext = new ExchangeDatabaseContext();
            _logger = logger;
            _config = configuration;

            _ = ExecuteRetentionPolicy();
        }

        /// <summary>
        /// Ctor of SecretEntity Repository without dependency-injected ISecretEntityRepository implementation
        /// </summary>
        /// <param name="logger">The DI implementation of ILogger should normally be used. Here, you can overrite it.</param>
        /// <param name="context">The DI implementation of ISecretEntityRepository should be used normally. Here, you can overrite it.</param>
        /// <param name="configuration">The DI implementation of IOptions\<MainSettings\> should be used normally. Here, you can overrite it.</param>
        public SecretEntityRepository(ILogger<SecretEntityRepository> logger, ExchangeDatabaseContext context, IOptions<MainSettings> configuration)
        {
            _dbContext = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger;
            _config = configuration;

            _ = ExecuteRetentionPolicy();
        }

        /// <inheritdoc/>
        public SecretEntity? GetSecretById(Guid secretId)
        {
            return _dbContext.Secrets.Find(secretId);
        }

        /// <inheritdoc/>
        public bool ExistsSecret(Guid secretId)
        {
            return _dbContext.Secrets.Find(secretId) != null;
        }

        /// <inheritdoc/>
        public SecretEntity AddSecret(SecretEntity secret)
        {
            if (secret == null)
            {
                throw new ArgumentNullException(nameof(secret));
            }
            
            var resultEntity = _dbContext.Secrets.Add(secret);
            _dbContext.SaveChanges();

            return resultEntity.Entity;
        }

        /// <inheritdoc/>
        public void DeleteSecret(Guid secretId)
        {
            var existingSecret = _dbContext.Set<SecretEntity>().Find(secretId);
            if (existingSecret != null)
            {
                _dbContext.Secrets.Remove(existingSecret);
                _dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Uses the timespan configuration of "appSettings.json > Main > Retention Span" to delete all older entries directly from the database.
        /// The execution is kept in the logging on information level, in case of deleting data.
        /// </summary>
        private async Task ExecuteRetentionPolicy()
        {
            var retention = _config.Value.RetentionSpan;
            if (retention.Ticks > 0)
            {
                var retentionDateTime = DateTime.UtcNow.Subtract(retention);
                var result = await _dbContext.Secrets.Where(e => e.CreatedDate < retentionDateTime.ToUniversalTime()).ExecuteDeleteAsync();

                if(result > 0)
                {
                    _logger.LogInformation("ExecuteRetentionPolicy with policy '{retention}' removes '{result}' rows.", retention, result);
                }
            }
        }
    }
}
