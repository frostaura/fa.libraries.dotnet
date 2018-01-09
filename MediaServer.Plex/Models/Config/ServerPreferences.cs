using System.Collections.Generic;
using System.Diagnostics;
using FrostAura.Libraries.Core.Attributes.Validation;

namespace MediaServer.Plex.Models.Config
{
    /// <summary>
    /// Where Plex server preferences get stored.
    /// </summary>
    [DebuggerDisplay("Count: {Size}")]
    public class ServerPreferences
    {
        /// <summary>
        /// Configuration size.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Collection of settings the length of 'Size'.
        /// </summary>
        [CollectionValidation(AllowEmptyCollection = false)]
        public IEnumerable<Setting> Setting { get; set; }
    }
}