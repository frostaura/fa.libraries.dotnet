using System.Collections.Generic;
using System.Xml.Serialization;

namespace MediaServer.Plex.Models.Content
{
    /// <summary>
    /// Devices XML response.
    /// </summary>
    [XmlRoot("MediaContainer")]
    public class DevicesMediaContainer
    {
        /// <summary>
        /// Public container address.
        /// </summary>
        [XmlAttribute("publicAddress")]
        public string PublicAddress { get; set; }

        /// <summary>
        /// Devices collection for the response.
        /// </summary>
        [XmlElement("Device")]
        public List<Device> Devices { get; set; } = new List<Device>();
    }
}