namespace AnnOtter.WayToSecureExchange.Models.API.Exchange
{
    /// <summary>
    /// Confirmation model
    /// </summary>
    public class UploadConfirmationModel
    {
        /// <summary>
        /// Unique identifier of the secret data object.
        /// </summary>
        public string? UploadId { get; set; }

        /// <summary>
        /// Upload time of secret data object.
        /// </summary>
        public DateTime? UploadTimestamp { get; set; }

        /// <summary>
        /// Hash value of the stored secret data object.
        /// </summary>
        public string? UploadOriginHash { get; set; }
        
        /// <summary>
        /// Upload status.
        /// </summary>
        public UploadState UploadState { get; set; }

        /// <summary>
        /// Human-readable processing message.
        /// </summary>
        public string? UploadMessage { get; set; }
    }
}
