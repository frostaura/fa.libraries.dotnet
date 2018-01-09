using System.Diagnostics;

namespace MediaServer.Core.Models.Content
{
    /// <summary>
    /// Movie container.
    /// </summary>
    [DebuggerDisplay("{Title}")]
    public class Movie
    {
        /// <summary>
        /// Movie title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Movie studio.
        /// </summary>
        public string Studio { get; set; }

        /// <summary>
        /// Movie description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Movie rating
        /// </summary>
        public double Rating { get; set; }

        /// <summary>
        /// Count of movie watches.
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// Year of release.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Full thumbnail URL.
        /// </summary>
        public string Thumbnail { get; set; }

        /// <summary>
        /// Movie poster full URL.
        /// </summary>
        public string Poster { get; set; }

        /// <summary>
        /// Duration in milliseconds.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Movie bitrate.
        /// </summary>
        public int Bitrate { get; set; }

        /// <summary>
        /// Movie frame width.
        /// </summary>
        public int Width { get; set; }
        
        /// <summary>
        /// Movie frame height.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Count of audio channels.
        /// </summary>
        public int AudioChannels { get; set; }

        /// <summary>
        /// Audio codec.
        /// </summary>
        public string AudioCodec { get; set; }
        
        /// <summary>
        /// Video codec.
        /// </summary>
        public string VideoCodec { get; set; }

        /// <summary>
        /// Video streaming url.
        /// </summary>
        public string StreamingUrl { get; set; }

        /// <summary>
        /// Media file container.
        /// </summary>
        public string Container { get; set; }
    }
}