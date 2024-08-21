namespace AnnOtter.WayToSecureExchange.Models.API.Exchange
{
    /// <summary>
    /// Represents the model for the upload data
    /// </summary>
    public class UploadDataModel
    {
        /// <summary>
        /// Contains the ciphertext to be uploaded.
        /// This encrypted data is sent by the client.
        /// </summary>
        public string? Data { get; set; }
    }
}
