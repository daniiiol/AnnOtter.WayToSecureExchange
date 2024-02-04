using AnnOtter.WayToSecureExchange.Configuration.AppSettings;
using Microsoft.Extensions.Options;

namespace AnnOtter.WayToSecureExchange.Middleware
{
    /// <summary>
    /// Middleware for adding security-related HTTP response headers to improve the security of the web application.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SecurityHeadersMiddleware"/> class.
    /// </remarks>
    /// <param name="_next">The next middleware in the pipeline.</param>
    public class SecurityHeadersMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        /// <summary>
        /// Adds various security-related headers to the HTTP response based on the provided configuration.
        /// </summary>
        /// <param name="context">The HttpContext for the current request, allowing access to request and response metadata and content.</param>
        /// <param name="logger">An ILogger instance for logging errors that occur during the processing of adding security headers.</param>
        /// <param name="securityHeadersConfig">Configuration options for security headers, wrapped in IOptions for dependency injection.</param>
        /// <returns>A Task representing the asynchronous operation of processing the request and adding security headers to the response.</returns>
        public async Task Invoke(HttpContext context, ILogger<SecurityHeadersMiddleware> logger, IOptions<SecurityHeaderSettings> securityHeadersConfig)
        {
            try
            {
                var headers = context.Response.Headers;

                if (!string.IsNullOrEmpty(securityHeadersConfig.Value.ContentSecurityPolicy))
                {
                    headers.Add("Content-Security-Policy", securityHeadersConfig.Value.ContentSecurityPolicy);
                }

                if (!string.IsNullOrEmpty(securityHeadersConfig.Value.XFrameOptions))
                {
                    headers.Add("X-Frame-Options", securityHeadersConfig.Value.XFrameOptions);
                }

                if (!string.IsNullOrEmpty(securityHeadersConfig.Value.XSSProtection))
                {
                    headers.Add("X-XSS-Protection", securityHeadersConfig.Value.XSSProtection);
                }

                if (!string.IsNullOrEmpty(securityHeadersConfig.Value.ContentTypeOptions))
                {
                    headers.Add("X-Content-Type-Options", securityHeadersConfig.Value.ContentTypeOptions);
                }

                if (!string.IsNullOrEmpty(securityHeadersConfig.Value.StrictTransportSecurity))
                {
                    headers.Add("Strict-Transport-Security", securityHeadersConfig.Value.StrictTransportSecurity);
                }

                if (!string.IsNullOrEmpty(securityHeadersConfig.Value.ReferrerPolicy))
                {
                    headers.Add("Referrer-Policy", securityHeadersConfig.Value.ReferrerPolicy);
                }

                if (!string.IsNullOrEmpty(securityHeadersConfig.Value.PermissionsPolicy))
                {
                    headers.Add("Permissions-Policy", securityHeadersConfig.Value.PermissionsPolicy);
                }

                if (!string.IsNullOrEmpty(securityHeadersConfig.Value.CrossOriginEmbedderPolicy))
                {
                    headers.Add("Cross-Origin-Embedder-Policy", securityHeadersConfig.Value.CrossOriginEmbedderPolicy);
                }

                if (!string.IsNullOrEmpty(securityHeadersConfig.Value.CrossOriginOpenerPolicy))
                {
                    headers.Add("Cross-Origin-Opener-Policy", securityHeadersConfig.Value.CrossOriginOpenerPolicy);
                }

                // Clean up.
                headers.Remove("Server");
                headers.Remove("X-Powered-By");
                headers.Remove("x-aspnet-version");
            }
            catch(Exception ex)
            {
                logger.LogError("{ex.Message}", ex.Message);
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
