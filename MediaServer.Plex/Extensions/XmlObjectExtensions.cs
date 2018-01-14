using System;
using System.IO;
using System.Xml.Serialization;

namespace MediaServer.Plex.Extensions
{
    /// <summary>
    /// Object extensions for XML strings.
    /// </summary>
    public static class XmlObjectExtensions
    {
        /// <summary>
        /// Deserialize an object from a XML string.
        /// </summary>
        /// <param name="xmlString">XML string.</param>
        /// <typeparam name="T">Type of object to deserialize to.</typeparam>
        /// <returns>Deserialized object.</returns>
        public static T FromXmlString<T>(this string xmlString)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
            
                using (TextReader reader = new StringReader(xmlString))
                {
                    return (T) serializer.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                return default(T);
            }
        }
    }
}