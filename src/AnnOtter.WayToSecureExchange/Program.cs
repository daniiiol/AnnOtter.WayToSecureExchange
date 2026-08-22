using AnnOtter.WayToSecureExchange.Configuration.AppSettings;
using AnnOtter.WayToSecureExchange.Databases;
using AnnOtter.WayToSecureExchange.HealthChecks;
using AnnOtter.WayToSecureExchange.Middleware;
using AnnOtter.WayToSecureExchange.Repositories;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ExchangeDatabaseContext>();
builder.Services.AddScoped<ISecretEntityRepository, SecretEntityRepository>();
builder.Services.Configure<EncryptionSettings>(builder.Configuration.GetSection("Encryption"));
builder.Services.Configure<MainSettings>(builder.Configuration.GetSection("Main"));
builder.Services.Configure<LabelsSettings>(builder.Configuration.GetSection("Labels"));
builder.Services.Configure<AppearanceSettings>(builder.Configuration.GetSection("Appearance"));
builder.Services.Configure<SecurityHeaderSettings>(builder.Configuration.GetSection("SecurityHeaders"));
builder.WebHost.UseKestrel(option => option.AddServerHeader = false);

var rateLimiterOptions = new RateLimiterOptionsSettings();
builder.Configuration.GetSection("RateLimiter").Bind(rateLimiterOptions);

builder.Services.AddHealthChecks()
.AddCheck<DatabaseHealthCheck>(name: "database", tags: ["db", "ready"])
.AddCheck<ApplicationHealthCheck>(name: "application", tags: ["app", "ready"]);

builder.Services.AddRateLimiter(_ =>
{
    _.AddPolicy("healthcheck", _ =>
        RateLimitPartition.GetFixedWindowLimiter("healthcheck", _ =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromSeconds(10),
                AutoReplenishment = true
            }));
    _.OnRejected = (context, _) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken: _);

        return new ValueTask();
    };
    _.GlobalLimiter = PartitionedRateLimiter.CreateChained(
        PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        {
            if (httpContext.Request.Path.StartsWithSegments("/health"))
                return RateLimitPartition.GetNoLimiter<string>("health-bypass");

            var userAgent = httpContext.Request.Headers.UserAgent.ToString();

            return RateLimitPartition.GetFixedWindowLimiter
            (userAgent, _ =>
                new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = rateLimiterOptions.ShortBurstAutoReplenishment,
                    PermitLimit = rateLimiterOptions.ShortBurstPermitLimit,
                    Window = TimeSpan.FromSeconds(rateLimiterOptions.ShortBurstWindow)
                });
        }),
        PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        {
            if (httpContext.Request.Path.StartsWithSegments("/health"))
                return RateLimitPartition.GetNoLimiter<string>("health-bypass");

            var userAgent = httpContext.Request.Headers.UserAgent.ToString();

            return RateLimitPartition.GetFixedWindowLimiter
            (userAgent, _ =>
                new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = rateLimiterOptions.GeneralAutoReplenishment,
                    PermitLimit = rateLimiterOptions.GeneralPermitLimit,
                    Window = TimeSpan.FromSeconds(rateLimiterOptions.GeneralWindow)
                });
        }));
});

builder.Configuration.AddEnvironmentVariables();


var loggerFactory = LoggerFactory.Create(
    builder => builder
                .AddConsole()
                .AddDebug()
                .SetMinimumLevel(LogLevel.Debug)
);

ILogger logger = loggerFactory.CreateLogger<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    logger.LogInformation("Attention: DevMode active!");
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSecurityHeaders();

app.UseThemeMiddleware();

app.UseRateLimiter();

app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    // Explicit status codes for (e.g.) Kubernetes liveness/readiness probes:
    // Degraded (200) keeps the pod in rotation; Unhealthy (503) triggers a restart.
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    },
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
        checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                message = e.Value.Description
            })
        });
        await context.Response.WriteAsync(result);
    }
}).RequireRateLimiting("healthcheck");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

logger.LogInformation("Application started.");
app.Run();

public partial class Program { }