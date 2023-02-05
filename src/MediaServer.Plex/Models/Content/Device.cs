using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;

namespace MediaServer.Plex.Models.Content
{
    /// <summary>
    /// Devices model from Plex.tv
    /// </summary>
    [DebuggerDisplay("{Name} - {PublicAddress}")]
    public class Device
    {
        /// <summary>
        /// Unique client identifier.
        /// </summary>
        [XmlAttribute("clientIdentifier")]
        public string ClientIdentifier { get; set; }
        
        /// <summary>
        /// Device identifier.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// Public device IP address.
        /// </summary>
        [XmlAttribute("publicAddress")]
        public string PublicAddress { get; set; }
        
        /// <summary>
        /// Plex product running on the device, description.
        /// </summary>
        [XmlAttribute("product")]
        public string Product { get; set; }
        
        /// <summary>
        /// Plex product running on the device, version.
        /// </summary>
        [XmlAttribute("productVersion")]
        public string ProductVersion { get; set; }
        
        /// <summary>
        /// Platform the device is running on.
        /// </summary>
        [XmlAttribute("platform")]
        public string Platform { get; set; }
        
        /// <summary>
        /// Platform version the device is running on.
        /// </summary>
        [XmlAttribute("platformVersion")]
        public string PlatformVersion { get; set; }
        
        /// <summary>
        /// Device model.
        /// </summary>
        [XmlAttribute("model")]
        public string Model { get; set; }
        
        /// <summary>
        /// Manufacturer of the device.
        /// </summary>
        [XmlAttribute("vendor")]
        public string Vendor { get; set; }
        
        /// <summary>
        /// What Plex service the device provides, if any.
        /// E.g. 'server'
        /// </summary>
        [XmlAttribute("provides")]
        public string Provides { get; set; }
        
        /// <summary>
        /// Device version.
        /// </summary>
        [XmlAttribute("version")]
        public string Version { get; set; }
        
        /// <summary>
        /// Device unique identifier.
        /// </summary>
        [XmlAttribute("id")]
        public string Id { get; set; }
        
        /// <summary>
        /// Device conenction token.
        /// </summary>
        [XmlAttribute("token")]
        public string Token { get; set; }
        
        /// <summary>
        /// When the device first joined the Plex network.
        /// </summary>
        [XmlAttribute("createdAt")]
        public long CreatedAt { get; set; }
        
        /// <summary>
        /// When last the device was online the Plex network.
        /// </summary>
        [XmlAttribute("lastSeenAt")]
        public long LastSeenAt { get; set; }
        
        /// <summary>
        /// Device supported resoutions, if device is a Plex client.
        /// </summary>
        [XmlAttribute("screenResolution")]
        public string ScreenResolution { get; set; }
        
        /// <summary>
        /// Device supported density, if device is a Plex client.
        /// </summary>
        [XmlAttribute("screenDensity")]
        public string ScreenDensity { get; set; }
        
        /// <summary>
        /// Device active connections.
        /// </summary>
        [XmlElement("Connection")]
        public List<Connection> Connections { get; set; } = new List<Connection>();

        /// <summary>
        /// Grab the local connection for the device by eliminating the public facing one.
        /// </summary>
        public Connection LocalConnection => Connections?.FirstOrDefault(c => !c.Uri.Equals(PublicAddress));
    }
}