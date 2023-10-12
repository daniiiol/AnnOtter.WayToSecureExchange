using AnnOtter.WayToSecureExchange.Configuration.AppSettings;
using Microsoft.Extensions.Options;

namespace AnnOtter.WayToSecureExchange.Middleware
{
    /// <summary>
    /// Middleware for adding security-related HTTP response headers to improve the security of the web application.
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityHeadersMiddleware> _logger;
        private readonly IOptions<SecurityHeaderSettings> _securityHeadersConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityHeadersMiddleware"/> class.
        /// </summary>
        /// <param name="_next">The next middleware in the pipeline.</param>
        /// <param name="_logger">DI injected logger.</param>
        /// <param name="_securityHeadersConfig">DI injected security headers config.</param>
        public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger, IOptions<SecurityHeaderSettings> securityHeadersConfiguration)
        {
            this._logger = logger;
            this._securityHeadersConfig = securityHeadersConfiguration;
            _next = next;
        }

        /// <summary>
        /// Invokes the middleware for adding security-related HTTP response headers.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                var headers = context.Response.Headers;

                if (!string.IsNullOrEmpty(_securityHeadersConfig.Value.ContentSecurityPolicy))
                {
                    headers.Add("Content-Security-Policy", _securityHeadersConfig.Value.ContentSecurityPolicy);
                }

                if (!string.IsNullOrEmpty(_securityHeadersConfig.Value.XFrameOptions))
                {
                    headers.Add("X-Frame-Options", _securityHeadersConfig.Value.XFrameOptions);
                }

                if (!string.IsNullOrEmpty(_securityHeadersConfig.Value.XSSProtection))
                {
                    headers.Add("X-XSS-Protection", _securityHeadersConfig.Value.XSSProtection);
                }

                if (!string.IsNullOrEmpty(_securityHeadersConfig.Value.ContentTypeOptions))
                {
                    headers.Add("X-Content-Type-Options", _securityHeadersConfig.Value.ContentTypeOptions);
                }

                if (!string.IsNullOrEmpty(_securityHeadersConfig.Value.StrictTransportSecurity))
                {
                    headers.Add("Strict-Transport-Security", _securityHeadersConfig.Value.StrictTransportSecurity);
                }

                if (!string.IsNullOrEmpty(_securityHeadersConfig.Value.ReferrerPolicy))
                {
                    headers.Add("Referrer-Policy", _securityHeadersConfig.Value.ReferrerPolicy);
                }

                if (!string.IsNullOrEmpty(_securityHeadersConfig.Value.PermissionsPolicy))
                {
                    headers.Add("Permissions-Policy", _securityHeadersConfig.Value.PermissionsPolicy);
                }

                if (!string.IsNullOrEmpty(_securityHeadersConfig.Value.CrossOriginEmbedderPolicy))
                {
                    headers.Add("Cross-Origin-Embedder-Policy", _securityHeadersConfig.Value.CrossOriginEmbedderPolicy);
                }

                if (!string.IsNullOrEmpty(_securityHeadersConfig.Value.CrossOriginOpenerPolicy))
                {
                    headers.Add("Cross-Origin-Opener-Policy", _securityHeadersConfig.Value.CrossOriginOpenerPolicy);
                }

                // Clean up.
                headers.Remove("Server");
                headers.Remove("X-Powered-By");
                headers.Remove("x-aspnet-version");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            await _next.Invoke(context);
        }
    }

    /// <summary>
    /// Extension class for configuring and using the <see cref="SecurityHeadersMiddleware"/> in the ASP.NET Core pipeline.
    /// </summary>
    public static class SecurityHeadersMiddlewareExtensions
    {
        /// <summary>
        /// Adds the <see cref="SecurityHeadersMiddleware"/> to the ASP.NET Core application's middleware pipeline.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to configure.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> with the <see cref="SecurityHeadersMiddleware"/> added.</returns>
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<SecurityHeadersMiddleware>();
        }

    }
}
