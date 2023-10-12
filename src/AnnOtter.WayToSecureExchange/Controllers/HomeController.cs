using AnnOtter.WayToSecureExchange.Configuration.AppSettings;
using AnnOtter.WayToSecureExchange.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace AnnOtter.WayToSecureExchange.Controllers
{
    /// <summary>
    /// Start controller and default route of this application.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOptions<LabelsSettings> _labelConfig;
        private readonly IOptions<AppearanceSettings> _appearanceConfig;

        /// <summary>
        /// Ctor of HomeController.
        /// </summary>
        /// <param name="logger">DI injected logger framwork.</param>
        /// <param name="labelConfiguration">DI injected labels setting.</param>
        /// <param name="appearanceConfiguration">DI injected appearance setting.</param>
        public HomeController(ILogger<HomeController> logger, IOptions<LabelsSettings> labelConfiguration, IOptions<AppearanceSettings> appearanceConfiguration)
        {
            this._logger = logger;
            this._labelConfig = labelConfiguration;
            this._appearanceConfig = appearanceConfiguration;
        }

        /// <summary>
        /// Start page of this application.
        /// </summary>
        /// <returns>Secure Exchange startpage.</returns>
        public IActionResult Index()
        {
            var viewModel = new DefaultViewModel(_labelConfig.Value, _appearanceConfig.Value);
            return View(viewModel);
        }

        /// <summary>
        /// Errorcatcher defined as global exception handler in Program.cs.
        /// </summary>
        /// <returns>Error page with a unique request id.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            _logger.LogWarning($"Error executed with RequestId: {requestId}. Requested ressource: {HttpContext.Request.Path} and QueryString: {HttpContext.Request.QueryString}");
            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}