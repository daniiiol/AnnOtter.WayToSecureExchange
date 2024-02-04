using AnnOtter.WayToSecureExchange.Configuration.AppSettings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace AnnOtter.WayToSecureExchange.Middleware
{
    /// <summary>
    /// Middleware for processing requests to dynamically replace CSS theme color placeholders
    /// with actual values defined in application settings, and for optimizing content delivery
    /// using HTTP caching techniques.
    /// </summary>
    /// <param name="next">The next middleware in the request pipeline.</param>
    public class ThemeMiddleware(RequestDelegate next)
    {
        private const string THEME_CACHE_KEY = "theme_css_content";
        private readonly RequestDelegate _next = next;

        /// <summary>
        /// Processes HTTP requests targeting "/css/theme.css" by dynamically replacing placeholders in the CSS template
        /// with color values from appearance settings and leveraging ETags and caching for optimized content delivery.
        /// </summary>
        /// <param name="context">The HttpContext for the current request, providing access to the request and response.</param>
        /// <param name="webHostEnvironment">Provides information about the web hosting environment, used here to locate the CSS template file.</param>
        /// <param name="logger">An ILogger instance used for logging errors that occur during the processing of the request.</param>
        /// <param name="appearanceSettings">The appearance settings, wrapped in IOptions, containing color values to replace in the CSS template.</param>
        /// <param name="cache">An IMemoryCache instance used to store and retrieve the processed CSS content and its ETag, optimizing performance for subsequent requests.</param>
        /// <returns>A Task representing the asynchronous operation of processing the request and writing the response.</returns>
        public async Task Invoke(HttpContext context, IWebHostEnvironment webHostEnvironment, ILogger<ThemeMiddleware> logger, IOptions<AppearanceSettings> appearanceSettings, IMemoryCache cache)
        {
            try
            {
                if (context.Request.Path.StartsWithSegments("/css/theme.css"))
                {
                    if (!cache.TryGetValue(THEME_CACHE_KEY, out (string Content, string ETag)? cacheEntry))
                    {
                        var themeCssPath = Path.Join(webHostEnvironment.WebRootPath, "./css/theme_base.css");
                        var content = File.ReadAllText(themeCssPath);
                        content = ReplacePlaceholders(content, appearanceSettings.Value);
                        var eTag = GenerateETag(content);

                        cacheEntry = (content, eTag);
                        cache.Set(THEME_CACHE_KEY, cacheEntry);
                    }

                    context.Response.ContentType = "text/css";
                    context.Response.Headers.ETag = cacheEntry?.ETag;

                    if (context.Request.Headers.IfNoneMatch == cacheEntry?.ETag)
                    {
                        context.Response.StatusCode = StatusCodes.Status304NotModified;
                        return;
                    }

                    var response = cacheEntry?.Content;

                    if (!string.IsNullOrEmpty(response))
                    {
                        await context.Response.WriteAsync(response);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("{ex.Message}", ex.Message);
            }

            await _next.Invoke(context);
        }

        /// <summary>
        /// Generates an ETag (Entity Tag) for the provided content string.
        /// </summary>
        /// <param name="content">The content for which to generate the ETag. If null, an empty string is assumed.</param>
        /// <returns>
        /// A string representing the ETag, which is a quoted string of hexadecimal digits representing the SHA256 hash of the input content.
        /// The ETag is enclosed in double quotes (") as per the HTTP ETag format specification.
        /// </returns>
        private static string GenerateETag(string? content)
        {
            var contentBytes = System.Text.Encoding.UTF8.GetBytes(content ?? string.Empty);
            var hashBytes = System.Security.Cryptography.SHA256.HashData(contentBytes);

            return $"\"{BitConverter.ToString(hashBytes).Replace("-", "")}\"";
        }

        /// <summary>
        /// Replaces placeholders in a given content string with the actual color values from the provided appearance settings.
        /// </summary>
        /// <param name="content">The content containing placeholders to be replaced. Placeholders are expected to be in the format {{PlaceholderName}}.</param>
        /// <param name="appearanceSettings">An instance of <see cref="AppearanceSettings"/> containing the color settings to replace the placeholders with.</param>
        /// <returns>
        /// A new string with all placeholders replaced by their corresponding values from the appearance settings.
        /// </returns>
        private static string ReplacePlaceholders(string content, AppearanceSettings appearanceSettings)
        {
            content = content.Replace("{{PrimaryColor}}", appearanceSettings.PrimaryColor);
            content = content.Replace("{{SecondaryColor}}", appearanceSettings.SecondaryColor);
            content = content.Replace("{{PrimaryAccentColor}}", appearanceSettings.PrimaryAccentColor);
            content = content.Replace("{{SecondaryAccentColor}}", appearanceSettings.SecondaryAccentColor);
            content = content.Replace("{{TextColor}}", appearanceSettings.TextColor);
            content = content.Replace("{{SuccessColor}}", appearanceSettings.SuccessColor);
            content = content.Replace("{{SuccessAccentColor}}", appearanceSettings.SuccessAccentColor);
            content = content.Replace("{{InfoColor}}", appearanceSettings.InfoColor);
            content = content.Replace("{{InfoAccentColor}}", appearanceSettings.InfoAccentColor);
            content = content.Replace("{{WarningColor}}", appearanceSettings.WarningColor);
            content = content.Replace("{{WarningAccentColor}}", appearanceSettings.WarningAccentColor);
            content = content.Replace("{{CopyElementColor}}", appearanceSettings.CopyElementColor);
            content = content.Replace("{{CopyElementAccentColor}}", appearanceSettings.CopyElementAccentColor);
            content = content.Replace("{{HighlightColor}}", appearanceSettings.HighlightColor);
            content = content.Replace("{{HighlightAccentColor}}", appearanceSettings.HighlightAccentColor);
            content = content.Replace("{{CodeColor}}", appearanceSettings.CodeColor);

            return content;
        }
    }

    /// <summary>
    /// Extension class for configuring and using the <see cref="ThemeMiddleware"/> in the ASP.NET Core pipeline.
    /// </summary>
    public static class ThemeMiddlewareExtensions
    {
        /// <summary>
        /// Adds the <see cref="ThemeMiddleware"/> to the ASP.NET Core application's middleware pipeline.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to configure.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> with the <see cref="ThemeMiddleware"/> added.</returns>
        public static IApplicationBuilder UseThemeMiddleware(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<ThemeMiddleware>();
        }

    }
}