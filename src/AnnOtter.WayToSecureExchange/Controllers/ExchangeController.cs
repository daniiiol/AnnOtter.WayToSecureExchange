using AnnOtter.WayToSecureExchange.Configuration.AppSettings;
using AnnOtter.WayToSecureExchange.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AnnOtter.WayToSecureExchange.Controllers
{
    /// <summary>
    /// Views about the exchange of secrets
    /// </summary>
    public class ExchangeController : Controller
    {
        private readonly IOptions<LabelsSettings> _labelConfig;
        private readonly IOptions<AppearanceSettings> _appearanceConfig;
        private readonly IOptions<MainSettings> _mainConfig;

        /// <summary>
        /// Ctor of ExchangeController.
        /// </summary>
        /// <param name="labelConfiguration">DI injected labels setting.</param>
        /// <param name="appearanceConfiguration">DI injected appearance setting.</param>
        /// <param name="mainConfiguration">DI injected main setting.</param>
        public ExchangeController(IOptions<LabelsSettings> labelConfiguration, IOptions<AppearanceSettings> appearanceConfiguration, IOptions<MainSettings> mainConfiguration)
        {
            this._labelConfig = labelConfiguration;
            this._appearanceConfig = appearanceConfiguration;
            this._mainConfig = mainConfiguration;
        }

        /// <summary>
        /// Loads the Get-Secret View
        /// </summary>
        /// <returns>/Exchange/Index.cshtml</returns>
        public IActionResult Index()
        {
            var viewModel = new DefaultViewModel(_labelConfig.Value, _appearanceConfig.Value, _mainConfig.Value.MaxSecretSize);
            return View(viewModel);
        }
    }
}
