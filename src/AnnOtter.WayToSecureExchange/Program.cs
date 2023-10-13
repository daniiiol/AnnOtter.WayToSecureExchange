using AnnOtter.WayToSecureExchange.Configuration.AppSettings;
using AnnOtter.WayToSecureExchange.Databases;
using AnnOtter.WayToSecureExchange.Middleware;
using AnnOtter.WayToSecureExchange.Repositories;
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

builder.Services.AddRateLimiter(_ =>
{
    _.OnRejected = (context, _) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken: _);

        return new ValueTask();
    };
    _.GlobalLimiter = PartitionedRateLimiter.CreateChained(
        PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        {
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

app.UseRateLimiter();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

logger.LogInformation("Application started.");
app.Run();