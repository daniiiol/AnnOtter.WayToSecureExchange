using AnnOtter.WayToSecureExchange.Configuration.AppSettings;
using AnnOtter.WayToSecureExchange.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AnnOtter.WayToSecureExchange.Controllers
{
    /// <summary>
    /// Views about the url protection
    /// </summary>
    public class ProtectController : Controller
    {
        private readonly IOptions<LabelsSettings> _labelConfig;
        private readonly IOptions<AppearanceSettings> _appearanceConfig;

        /// <summary>
        /// Ctor of ProtectController.
        /// </summary>
        /// <param name="labelConfiguration">DI injected labels setting.</param>
        /// <param name="appearanceConfiguration">DI injected appearance setting.</param>
        public ProtectController(IOptions<LabelsSettings> labelConfiguration, IOptions<AppearanceSettings> appearanceConfiguration)
        {
            this._labelConfig = labelConfiguration;
            this._appearanceConfig = appearanceConfiguration;
        }

        /// <summary>
        /// Loads the Get-Secret View
        /// </summary>
        /// <returns>/Exchange/Index.cshtml</returns>
        public IActionResult Index()
        {
            var viewModel = new DefaultViewModel(_labelConfig.Value, _appearanceConfig.Value);
            return View(viewModel);
        }
    }
}
