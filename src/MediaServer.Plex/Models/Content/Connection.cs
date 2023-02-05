using System.Diagnostics;
using System.Xml.Serialization;

namespace MediaServer.Plex.Models.Content
{
    /// <summary>
    /// Device connection model.
    /// </summary>
    [DebuggerDisplay("Uri")]
    public class Connection
    {
        /// <summary>
        /// Connection URI.
        /// </summary>
        [XmlAttribute("uri")]
        public string Uri { get; set; }
    }
}