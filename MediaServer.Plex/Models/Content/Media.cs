using System.Collections.Generic;
using System.Diagnostics;

namespace MediaServer.Plex.Models.Content
{
    /// <summary>
    /// Media content model.
    /// </summary>
    [DebuggerDisplay("Id: {Id}")]
    public class Media
    {
        /// <summary>
        /// Video resolution in P.
        /// </summary>
        public string VideoResolution { get; set; }

        /// <summary>
        /// Identifier of the media
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Duration in milliseconds.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Bitrate of the media.
        /// </summary>
        public int Bitrate { get; set; }

        /// <summary>
        /// Media width.
        /// </summary>
        public int Width { get; set; }
        
        /// <summary>
        /// Media height.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Aspect ratio of the media.
        /// </summary>
        public double AspectRatio { get; set; }

        /// <summary>
        /// Audio channels count of the media.
        /// </summary>
        public int AudioChannels { get; set; }

        /// <summary>
        /// Media audio codec.
        /// </summary>
        public string AudioCodec { get; set; }
        
        /// <summary>
        /// Media video codec.
        /// </summary>
        public string VideoCodec { get; set; }

        /// <summary>
        /// Media container.
        /// </summary>
        public string Container { get; set; }

        /// <summary>
        /// Video framerate of the media.
        /// </summary>
        public string VideoFrameRate { get; set; }

        /// <summary>
        /// Whether or not the media is optimized for streaming.
        /// </summary>
        public int OptimizedForStreaming { get; set; }

        /// <summary>
        /// Profile of media audio.
        /// </summary>
        public string AudioProfile { get; set; }

        /// <summary>
        /// Whether or not the media has a 64 bit offset.
        /// </summary>
        public bool Has64bitOffsets { get; set; }

        /// <summary>
        /// Video profile quality.
        /// </summary>
        public string VideoProfile { get; set; }

        /// <summary>
        /// Collection of parts the media consists of.
        /// </summary>
        public IEnumerable<Part> Part { get; set; }
    }
}