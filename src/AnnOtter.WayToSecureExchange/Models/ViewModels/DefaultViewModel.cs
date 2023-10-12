using AnnOtter.WayToSecureExchange.Configuration.AppSettings;

namespace AnnOtter.WayToSecureExchange.Models.ViewModels
{
    /// <summary>
    /// Represents the default view model class for all renderings.
    /// </summary>
    public class DefaultViewModel
    {
        private readonly LabelsSettings _labelsSettings;
        private readonly AppearanceSettings _appearanceSettings;

        /// <summary>
        /// Complete LabelsSettings configuration.
        /// </summary>
        public LabelsSettings Labels { get => _labelsSettings; }
        
        /// <summary>
        /// Complete AppearanceSettings configuration.
        /// </summary>
        public AppearanceSettings Appearance { get => _appearanceSettings; }

        /// <summary>
        /// Ctor of default view model class.
        /// </summary>
        /// <param name="labelsSettings">LabelsSettings to get all text objects.</param>
        /// <param name="appearanceSettings">AppearanceSettings to make some elements customizable.</param>
        public DefaultViewModel(LabelsSettings labelsSettings, AppearanceSettings appearanceSettings) {
            this._labelsSettings = labelsSettings;
            this._appearanceSettings = appearanceSettings;
        }
    }
}
