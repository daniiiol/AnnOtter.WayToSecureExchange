namespace AnnOtter.WayToSecureExchange.Configuration.AppSettings
{
    /// <summary>
    /// Rate limiting options. See https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit?view=aspnetcore-7.0 for further information.
    /// </summary>
    public class RateLimiterOptionsSettings
    {
        /// <summary>
        /// 1. Chain (Short Burst Mode): Maximum number of permit counters that can be allowed in a window.
        /// </summary>
        public int ShortBurstPermitLimit { get; set; } = 6;

        /// <summary>
        /// 1. Chain (Short Burst Mode): Specifies the time window (seconds) that takes in the requests.
        /// </summary>
        public int ShortBurstWindow { get; set; } = 2;

        /// <summary>
        /// 1. Chain (Short Burst Mode): Specified whether the FixedWindowRateLimiter is automatically refresh counters or if someone else
        /// will be calling FixedWindowRateLimiter.TryReplenish to refresh counters.
        /// </summary>
        public bool ShortBurstAutoReplenishment { get; set; } = true;

        /// <summary>
        /// 2. Chain (General Mode): Maximum number of permit counters that can be allowed in a window.
        /// </summary>
        public int GeneralPermitLimit { get; set; } = 20;

        /// <summary>
        /// 2. Chain (General Mode): Specifies the time window (seconds) that takes in the requests.
        /// </summary>
        public int GeneralWindow { get; set; } = 30;

        /// <summary>
        /// 2. Chain (General Mode): Specified whether the FixedWindowRateLimiter is automatically refresh counters or if someone else
        /// will be calling FixedWindowRateLimiter.TryReplenish to refresh counters.
        /// </summary>
        public bool GeneralAutoReplenishment { get; set; } = true;
    }
}
