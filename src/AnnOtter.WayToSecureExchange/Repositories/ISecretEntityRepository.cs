using AnnOtter.WayToSecureExchange.Models.Database;

namespace AnnOtter.WayToSecureExchange.Repositories
{
    /// <summary>
    /// Interface for using a repository for managing the SecretEntity.
    /// </summary>
    public interface ISecretEntityRepository
    {
        /// <summary>
        /// Finds a SecretEntity with the given primary key.
        /// </summary>
        /// <param name="secretId">Primary key of the secrets table.</param>
        /// <returns>A SecretEntity</returns>
        public SecretEntity? GetSecretById(Guid secretId);
        
        /// <summary>
        /// Finds a SecretEntity with the given primary key.
        /// </summary>
        /// <param name="secretId">Primary key of the secrets table.</param>
        /// <returns>True if a secret with the specified primary key exists. Otherwise false is returned.</returns>
        public bool ExistsSecret(Guid secretId);

        /// <summary>
        /// Create a new secret object to the database
        /// </summary>
        /// <param name="secret">Instance of SecretEntity.</param>
        /// <returns>The created database representation of SecretEntity.</returns>
        /// <exception cref="ArgumentNullException">Param 'secret' must not be null.</exception>
        public SecretEntity AddSecret(SecretEntity secret);

        /// <summary>
        /// Removes a SecretEntity with the given primary key
        /// </summary>
        /// <param name="secretId">Primary key of the secrets table.</param>
        public void DeleteSecret(Guid secretId);
    }
}