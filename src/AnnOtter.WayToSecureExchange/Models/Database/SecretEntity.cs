using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AnnOtter.WayToSecureExchange.Models.Database
{
    /// <summary>
    /// This class contains information about the Secret object itself
    /// </summary>
    public class SecretEntity
    {
        /// <summary>
        /// Primary SecretId to identify a secret
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SecretId { get; set; }
        
        /// <summary>
        /// Stored ciphertext
        /// </summary>
        public required string Ciphertext { get; set; }
        
        /// <summary>
        /// Used Nonce to double-encrypt the ciphertext
        /// </summary>
        public required string Nonce { get; set; }

        /// <summary>
        /// Used Tag to double-encrypt the ciphertext
        /// </summary>
        public required string Tag { get; set; }

        /// <summary>
        /// Datetime of data storage
        /// </summary>
        public DateTime CreatedDate { get; set; }

    }
}
