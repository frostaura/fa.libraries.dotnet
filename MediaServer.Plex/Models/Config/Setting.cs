using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace MediaServer.Plex.Models.Config
{
    /// <summary>
    /// Setting model.
    /// </summary>
    [DebuggerDisplay("{Label} - {Value}")]
    public class Setting
    {
        /// <summary>
        /// Unique setting identifier.
        /// </summary>
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// Short description of the setting.
        /// </summary>
        [Required]
        public string Label { get; set; }

        /// <summary>
        /// Default value for the setting as a string.
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        /// Full description of the setting.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// The data type of the setting.
        /// bool | string | int
        /// </summary>
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// String value of the setting.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Whether or not the setting is hidden.
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// Whether or not the setting is an advanced one or simple.
        /// </summary>
        public bool Advanced { get; set; }

        /// <summary>
        /// Group the setting belongs to.
        /// </summary>
        public string Group { get; set; }
    }
}