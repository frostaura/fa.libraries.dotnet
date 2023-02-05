using FrostAura.Libraries.Core.Models.Graphics;

namespace FrostAura.Libraries.Core.Interfaces.Graphics
{
    /// <summary>
    /// Interface for all properties a theme should contain.
    /// </summary>
    public interface ITheme
    {
        /// <summary>
        /// Unique theme identifier.
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Primary color RGB and Hex values.
        /// Baseline value.
        /// </summary>
        RGB Primary { get; }
        string PrimaryHex { get; }

        /// <summary>
        /// Primary darker color RGB and Hex values.
        /// 1.5x the baseline value.
        /// </summary>
        RGB PrimaryDark { get; }
        string PrimaryDarkHex { get; }

        /// <summary>
        /// Primary lighter color RGB and Hex values.
        /// 0.5x the baseline value.
        /// </summary>
        RGB PrimaryLight { get; }
        string PrimaryLightHex { get; }

        /// <summary>
        /// Text and icons RGB and Hex values.
        /// </summary>
        RGB TextAndIcons { get; }
        string TextAndIconsHex { get; }

        /// <summary>
        /// Accent RGB and Hex values.
        /// </summary>
        RGB Accent { get; }
        string AccentHex { get; }

        /// <summary>
        /// Primary text color RGB and Hex values.
        /// 1.5x accent value.
        /// </summary>
        RGB PrimaryText { get; }
        string PrimaryTextHex { get; }

        /// <summary>
        /// Secondary text color RGB and Hex values.
        /// 0.5x accent value.
        /// </summary>
        RGB SecondaryText { get; }
        string SecondaryTextHex { get; }
    }
}