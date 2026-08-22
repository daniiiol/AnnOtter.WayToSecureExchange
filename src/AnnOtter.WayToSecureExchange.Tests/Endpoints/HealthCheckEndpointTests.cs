using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;

namespace AnnOtter.WayToSecureExchange.Tests.Endpoints
{
    [TestClass]
    public class HealthCheckEndpointTests
    {
        private static WebApplicationFactory<Program> _factory = null!;
        private static HttpClient _client = null!;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [TestMethod]
        public async Task HealthCheck_ReturnsHttpOk()
        {
            var response = await _client.GetAsync("/health");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task HealthCheck_ReturnsJsonContentType()
        {
            var response = await _client.GetAsync("/health");

            Assert.AreEqual("application/json", response.Content.Headers.ContentType?.MediaType);
        }

        [TestMethod]
        public async Task HealthCheck_ReturnsHealthyStatus()
        {
            var response = await _client.GetAsync("/health");
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<JsonElement>(content);

            Assert.AreEqual("Healthy", json.GetProperty("status").GetString());
        }

        [TestMethod]
        public async Task HealthCheck_ContainsDatabaseCheck()
        {
            var response = await _client.GetAsync("/health");
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<JsonElement>(content);

            var checks = json.GetProperty("checks").EnumerateArray().ToList();
            Assert.IsTrue(checks.Any(c => c.GetProperty("name").GetString() == "database"),
                "Expected a health check entry named 'database'.");
        }

        [TestMethod]
        public async Task HealthCheck_DatabaseCheck_IsHealthy()
        {
            var response = await _client.GetAsync("/health");
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<JsonElement>(content);

            var dbCheck = json.GetProperty("checks").EnumerateArray()
                .FirstOrDefault(c => c.GetProperty("name").GetString() == "database");

            Assert.AreEqual("Healthy", dbCheck.GetProperty("status").GetString());
        }

        [TestMethod]
        public async Task HealthCheck_DatabaseCheck_HasMessage()
        {
            var response = await _client.GetAsync("/health");
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<JsonElement>(content);

            var dbCheck = json.GetProperty("checks").EnumerateArray()
                .FirstOrDefault(c => c.GetProperty("name").GetString() == "database");

            var message = dbCheck.GetProperty("message").GetString();
            Assert.IsFalse(string.IsNullOrEmpty(message), "Expected a non-empty message for the database check.");
        }

        [TestMethod]
        public async Task HealthCheck_ContainsApplicationCheck()
        {
            var response = await _client.GetAsync("/health");
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<JsonElement>(content);

            var checks = json.GetProperty("checks").EnumerateArray().ToList();
            Assert.IsTrue(checks.Any(c => c.GetProperty("name").GetString() == "application"),
                "Expected a health check entry named 'application'.");
        }

        [TestMethod]
        public async Task HealthCheck_ApplicationCheck_IsNotUnhealthy()
        {
            var response = await _client.GetAsync("/health");
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<JsonElement>(content);

            var appCheck = json.GetProperty("checks").EnumerateArray()
                .FirstOrDefault(c => c.GetProperty("name").GetString() == "application");

            Assert.AreNotEqual("Unhealthy", appCheck.GetProperty("status").GetString());
        }
    }
}
