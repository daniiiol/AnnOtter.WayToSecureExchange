using AnnOtter.WayToSecureExchange.Configuration.AppSettings;
using AnnOtter.WayToSecureExchange.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace AnnOtter.WayToSecureExchange.Tests.Middleware
{

    [TestClass]
    public class SecurityHeadersMiddlewareTests
    {
        [TestMethod]
        public async Task Invoke_AddsSecurityHeadersToResponse()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var responseHeaders = context.Response.Headers;
            var securityHeadersConfig = new SecurityHeaderSettings
            {
                ContentSecurityPolicy = "default-src 'self';",
                XFrameOptions = "DENY",
                XSSProtection = "1; mode=block",
                ContentTypeOptions = "nosniff",
                StrictTransportSecurity = "max-age=31536000; includeSubDomains",
                ReferrerPolicy = "same-origin",
                PermissionsPolicy = "geolocation=()",
                CrossOriginEmbedderPolicy = "require-corp",
                CrossOriginOpenerPolicy = "same-origin",
            };
            var middleware = new SecurityHeadersMiddleware((innerContext) => Task.CompletedTask);

            var logger = Substitute.For<ILogger<SecurityHeadersMiddleware>>();
            var options = Substitute.For<IOptions<SecurityHeaderSettings>>();
            options.Value.Returns(securityHeadersConfig);

            // Act
            await middleware.Invoke(context, logger, options);

            // Assert
            Assert.AreEqual(securityHeadersConfig.ContentSecurityPolicy, responseHeaders["Content-Security-Policy"].ToString());
            Assert.AreEqual(securityHeadersConfig.XFrameOptions, responseHeaders["X-Frame-Options"].ToString());
            Assert.AreEqual(securityHeadersConfig.XSSProtection, responseHeaders["X-XSS-Protection"].ToString());
            Assert.AreEqual(securityHeadersConfig.ContentTypeOptions, responseHeaders["X-Content-Type-Options"].ToString());
            Assert.AreEqual(securityHeadersConfig.StrictTransportSecurity, responseHeaders["Strict-Transport-Security"].ToString());
            Assert.AreEqual(securityHeadersConfig.ReferrerPolicy, responseHeaders["Referrer-Policy"].ToString());
            Assert.AreEqual(securityHeadersConfig.PermissionsPolicy, responseHeaders["Permissions-Policy"].ToString());
            Assert.AreEqual(securityHeadersConfig.CrossOriginEmbedderPolicy, responseHeaders["Cross-Origin-Embedder-Policy"].ToString());
            Assert.AreEqual(securityHeadersConfig.CrossOriginOpenerPolicy, responseHeaders["Cross-Origin-Opener-Policy"].ToString());
        }

        [TestMethod]
        public async Task Invoke_CleansUpUnwantedHeaders()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var responseHeaders = context.Response.Headers;
            responseHeaders["Server"] = "TestServer";
            responseHeaders["X-Powered-By"] = "TestPoweredBy";
            responseHeaders["x-aspnet-version"] = "TestVersion";
            var securityHeadersConfig = new SecurityHeaderSettings();
            var middleware = new SecurityHeadersMiddleware((innerContext) => Task.CompletedTask);

            var logger = Substitute.For<ILogger<SecurityHeadersMiddleware>>();
            var options = Substitute.For<IOptions<SecurityHeaderSettings>>();
            options.Value.Returns(securityHeadersConfig);

            // Act
            await middleware.Invoke(context, logger, options);

            // Assert
            Assert.IsFalse(responseHeaders.ContainsKey("Server"));
            Assert.IsFalse(responseHeaders.ContainsKey("X-Powered-By"));
            Assert.IsFalse(responseHeaders.ContainsKey("x-aspnet-version"));
        }
    }

    [TestClass]
    public class SecurityHeadersMiddlewareExtensionsTests
    {
        [TestMethod]
        public void UseSecurityHeaders_ThrowsArgumentNullException_WhenAppIsNull()
        {
            // Arrange
            IApplicationBuilder? app = null;

            // Act and Assert
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                SecurityHeadersMiddlewareExtensions.UseSecurityHeaders(app!);
            });
        }

        [TestMethod]
        public void UseSecurityHeaders_ReturnsApplicationBuilder()
        {
            // Arrange
            var app = Substitute.For<IApplicationBuilder>();

            // Act
            var result = app.UseSecurityHeaders();

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
