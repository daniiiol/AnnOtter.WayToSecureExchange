namespace AnnOtter.WayToSecureExchange.Configuration.AppSettings
{
    /// <summary>
    /// Represents settings related to the appearance and branding of a web application.
    /// </summary>
    public class AppearanceSettings
    {
        private string? primaryAccentColor;
        private string? secondaryAccentColor;
        private string? textColor;
        private string? primaryColor;
        private string? secondaryColor;
        private string? highlightColor;
        private string? highlightAccentColor;
        private string? infoColor;
        private string? infoAccentColor;
        private string? warningColor;
        private string? warningAccentColor;
        private string? successColor;
        private string? successAccentColor;
        private string? copyElementColor;
        private string? copyElementAccentColor;
        private string? codeColor;

        /// <summary>
        /// Gets or sets a value indicating whether the application's logo should be displayed.
        /// </summary>
        public bool ShowLogo { get; set; }

        /// <summary>
        /// Gets or sets the path to the application's logo image file.
        /// </summary>
        public string? LogoPath { get; set; }

        /// <summary>
        /// PrimaryAccentColor: Defines the primary accent color used throughout the application.
        /// If the provided value does not constitute a valid HTML hex color code, it defaults to "#141518".
        /// </summary>
        public string? PrimaryAccentColor
        {
            get => IsValidHtmlHexCode(primaryAccentColor) ? primaryAccentColor : "#141518";
            set => primaryAccentColor = value;
        }

        /// <summary>
        /// SecondaryAccentColor: Specifies the secondary accent color.
        /// In case of an invalid HTML hex color code, it falls back to "#c6c7c8".
        /// </summary>
        public string? SecondaryAccentColor
        {
            get => IsValidHtmlHexCode(secondaryAccentColor) ? secondaryAccentColor : "#c6c7c8";
            set => secondaryAccentColor = value;
        }

        /// <summary>
        /// TextColor: Represents the color used for text elements. 
        /// If the provided value is not a valid HTML hex code, it defaults to "#afb5bc".
        /// </summary>
        public string? TextColor
        {
            get => IsValidHtmlHexCode(textColor) ? textColor : "#afb5bc";
            set => textColor = value;
        }

        /// <summary>
        /// PrimaryColor: Represents the primary color theme. 
        /// Defaults to "#222529" if the provided value is not a valid HTML hex code.
        /// </summary>
        public string? PrimaryColor
        {
            get => IsValidHtmlHexCode(primaryColor) ? primaryColor : "#222529";
            set => primaryColor = value;
        }

        /// <summary>
        /// SecondaryColor: Represents the secondary color theme. 
        /// Returns "#ffffff" if the provided value is not a valid HTML hex code.
        /// </summary>
        public string? SecondaryColor
        {
            get => IsValidHtmlHexCode(secondaryColor) ? secondaryColor : "#ffffff";
            set => secondaryColor = value;
        }

        /// <summary>
        /// HighlightColor: Used for highlighting elements. 
        /// Defaults to "#2c3035" for invalid HTML hex codes.
        /// </summary>
        public string? HighlightColor
        {
            get => IsValidHtmlHexCode(highlightColor) ? highlightColor : "#2c3035";
            set => highlightColor = value;
        }

        /// <summary>
        /// HighlightAccentColor: Accent color for highlights. 
        /// Falls back to "#42444a" if the value is not a valid HTML hex code.
        /// </summary>
        public string? HighlightAccentColor
        {
            get => IsValidHtmlHexCode(highlightAccentColor) ? highlightAccentColor : "#42444a";
            set => highlightAccentColor = value;
        }

        /// <summary>
        /// InfoColor: Color used for informational messages. 
        /// Defaults to "#f4ce6e" when provided with an invalid HTML hex code.
        /// </summary>
        public string? InfoColor
        {
            get => IsValidHtmlHexCode(infoColor) ? infoColor : "#f4ce6e";
            set => infoColor = value;
        }

        /// <summary>
        /// InfoAccentColor: Accent color for informational elements. 
        /// Uses "#ffbf00" for invalid HTML hex codes.
        /// </summary>
        public string? InfoAccentColor
        {
            get => IsValidHtmlHexCode(infoAccentColor) ? infoAccentColor : "#ffbf00";
            set => infoAccentColor = value;
        }

        /// <summary>
        /// WarningColor: Color used for warnings. 
        /// Invalid HTML hex codes will default to "#db7093".
        /// </summary>
        public string? WarningColor
        {
            get => IsValidHtmlHexCode(warningColor) ? warningColor : "#db7093";
            set => warningColor = value;
        }

        /// <summary>
        /// WarningAccentColor: Accent color for warnings. 
        /// Returns "#cc3363" for invalid HTML hex codes.
        /// </summary>
        public string? WarningAccentColor
        {
            get => IsValidHtmlHexCode(warningAccentColor) ? warningAccentColor : "#cc3363";
            set => warningAccentColor = value;
        }

        /// <summary>
        /// SuccessColor: Represents success messages color. 
        /// Defaults to "#32cd32" if the value is not a valid HTML hex code.
        /// </summary>
        public string? SuccessColor
        {
            get => IsValidHtmlHexCode(successColor) ? successColor : "#32cd32";
            set => successColor = value;
        }

        /// <summary>
        /// SuccessAccentColor: Accent color for success messages. 
        /// Uses "#7cfc00" for invalid HTML hex codes.
        /// </summary>
        public string? SuccessAccentColor
        {
            get => IsValidHtmlHexCode(successAccentColor) ? successAccentColor : "#7cfc00";
            set => successAccentColor = value;
        }

        /// <summary>
        /// CopyElementColor: Color for copyable text or elements. 
        /// Defaults to "#bc8f8f" if the value is not a valid HTML hex code.
        /// </summary>
        public string? CopyElementColor
        {
            get => IsValidHtmlHexCode(copyElementColor) ? copyElementColor : "#bc8f8f";
            set => copyElementColor = value;
        }

        /// <summary>
        /// CopyElementAccentColor: Accent color for copyable elements. 
        /// Uses "#f4a460" for invalid HTML hex codes.
        /// </summary>
        public string? CopyElementAccentColor
        {
            get => IsValidHtmlHexCode(copyElementAccentColor) ? copyElementAccentColor : "#f4a460";
            set => copyElementAccentColor = value;
        }

        /// <summary>
        /// CodeColor: Color used specifically for code snippets or elements. 
        /// Defaults to "#e685b5" when provided with an invalid HTML hex code.
        /// </summary>
        public string? CodeColor
        {
            get => IsValidHtmlHexCode(codeColor) ? codeColor : "#e685b5";
            set => codeColor = value;
        }

        /// <summary>
        /// Validates whether the provided string is a valid HTML hex color code.
        /// </summary>
        /// <param name="hexCode">The hex code string to validate. Can be null or empty.</param>
        /// <returns>
        /// <c>true</c> if the string is a valid HTML hex color code; otherwise, <c>false</c>.
        /// A valid HTML hex color code starts with a hash (#) followed by exactly 3 or 6 hexadecimal characters.
        /// </returns>
        private static bool IsValidHtmlHexCode(string? hexCode)
        {
            if (string.IsNullOrEmpty(hexCode))
            {
                return false;
            }
            if (!hexCode.StartsWith("#"))
            {
                hexCode = "#" + hexCode;
            }

            return System.Text.RegularExpressions.Regex.IsMatch(hexCode, "^#[0-9A-Fa-f]{3}([0-9A-Fa-f]{3})?$");
        }
    }
}
