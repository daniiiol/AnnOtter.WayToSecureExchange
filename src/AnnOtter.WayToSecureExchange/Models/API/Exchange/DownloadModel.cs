namespace AnnOtter.WayToSecureExchange.Models.API.Exchange
{
    /// <summary>
    /// Secret Exchange model, which the recipient downloads.
    /// </summary>
    public class DownloadModel
    {
        /// <summary>
        /// Unique identifier of the secret data object.
        /// </summary>
        public required string UploadId { get; set; }
        
        /// <summary>
        /// Calculated SHA-256 hash of the secret data object on server side.
        /// </summary>
        public required string Hash { get; set; }
        
        /// <summary>
        /// Secret data object.
        /// </summary>
        public required string Data { get; set; }
    }
}
