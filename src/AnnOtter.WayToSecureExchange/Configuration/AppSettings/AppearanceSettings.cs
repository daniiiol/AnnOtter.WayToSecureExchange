namespace AnnOtter.WayToSecureExchange.Configuration.AppSettings
{
    /// <summary>
    /// Represents settings related to the appearance and branding of a web application.
    /// </summary>
    public class AppearanceSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether the application's logo should be displayed.
        /// </summary>
        public bool ShowLogo { get; set; }
        
        /// <summary>
        /// Gets or sets the path to the application's logo image file.
        /// </summary>
        public string? LogoPath { get; set; }
    }
}
