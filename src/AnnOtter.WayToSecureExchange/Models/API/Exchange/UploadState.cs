namespace AnnOtter.WayToSecureExchange.Models.API.Exchange
{
    /// <summary>
    /// Statuses that can happen during the upload.
    /// </summary>
    public enum UploadState
    {
        /// <summary>
        /// Default value.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Upload was successful
        /// </summary>
        Succeeded = 1,

        /// <summary>
        /// Upload has failed.
        /// </summary>
        Failed = 2
    }
}
