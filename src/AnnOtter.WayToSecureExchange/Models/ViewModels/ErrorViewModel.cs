namespace AnnOtter.WayToSecureExchange.Models.ViewModels
{
    /// <summary>
    /// ASP.NET Default ErrorViewModel
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Unique request ID of the HTTP-Request.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Boolean whether to display the RequestId.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}