namespace AnnOtter.WayToSecureExchange.Configuration.AppSettings
{
    /// <summary>
    /// Represents security header settings for HTTP responses, including various security-related headers.
    /// </summary>
    public class SecurityHeaderSettings
    {
        /// <summary>
        /// Gets or sets the Content Security Policy (CSP) header value, which helps prevent cross-site scripting (XSS) attacks.
        /// </summary>
        public string? ContentSecurityPolicy { get; set; }

        /// <summary>
        /// Gets or sets the X-Frame-Options header value, which helps prevent clickjacking attacks.
        /// </summary>
        public string? XFrameOptions { get; set; }

        /// <summary>
        /// Gets or sets the X-XSS-Protection header value, which enables or disables the built-in browser XSS protection.
        /// </summary>
        public string? XSSProtection { get; set; }

        /// <summary>
        /// Gets or sets the X-Content-Type-Options header value, which prevents content type sniffing.
        /// </summary>
        public string? ContentTypeOptions { get; set; }

        /// <summary>
        /// Gets or sets the Strict Transport Security (HSTS) header value, which enforces secure connections.
        /// </summary>
        public string? StrictTransportSecurity { get; set; }

        /// <summary>
        /// Gets or sets the Referrer Policy header value, which controls the information sent with requests.
        /// </summary>
        public string? ReferrerPolicy { get; set; }

        /// <summary>
        /// Gets or sets the Permissions Policy header value, which controls permissions for web features.
        /// </summary>
        public string? PermissionsPolicy { get; set; }

        /// <summary>
        /// Gets or sets the Cross-Origin Embedder Policy (COEP) header value, which controls embedded cross-origin content.
        /// </summary>
        public string? CrossOriginEmbedderPolicy { get; set; }

        /// <summary>
        /// Gets or sets the Cross-Origin Opener Policy (COOP) header value, which controls how documents may be opened in a browsing context.
        /// </summary>
        public string? CrossOriginOpenerPolicy { get; set; }
    }
}
