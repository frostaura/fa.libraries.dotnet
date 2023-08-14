using System;
using System.Diagnostics;
using System.Numerics;

namespace MediaServer.Plex.Models.Content
{
    /// <summary>
    /// Part of which media consists.
    /// </summary>
    [DebuggerDisplay("{File}")]
    public class Part
    {
        /// <summary>
        /// Identifier of the media.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Part key.
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// Duration in milliseconds.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// File part of the media.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// The size of the file for the part in KB.
        /// </summary>
        public Int64 Size { get; set; }

        /// <summary>
        /// Memory profile of the part.
        /// </summary>
        public string MemoryProfile { get; set; }
        
        /// <summary>
        /// Media container.
        /// </summary>
        public string Container { get; set; }
        
        /// <summary>
        /// Whether or not the media has a 64 bit offset.
        /// </summary>
        public bool Has64bitOffsets { get; set; }
        
        /// <summary>
        /// Video profile quality.
        /// </summary>
        public string VideoProfile { get; set; }
    }
}