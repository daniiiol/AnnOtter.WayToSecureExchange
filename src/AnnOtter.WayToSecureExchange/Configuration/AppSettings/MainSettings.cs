namespace AnnOtter.WayToSecureExchange.Configuration.AppSettings
{
    /// <summary>
    /// General settings around this application.
    /// </summary>
    public class MainSettings
    {
        /// <summary>
        /// Time span until a secret exchange should be deleted.
        /// </summary>
        public TimeSpan RetentionSpan { get; set; }
    }
}
