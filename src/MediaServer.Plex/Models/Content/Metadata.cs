using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MediaServer.Plex.Models.Content
{
    /// <summary>
    /// Media content model.
    /// </summary>
    [DebuggerDisplay("Title: {Title}")]
    public class Metadata
    {
        /// <summary>
        /// Media rating key.
        /// </summary>
        public string RatingKey { get; set; }

        /// <summary>
        /// The metadata key of the media.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Studio that produced the media.
        /// </summary>
        public string Studio { get; set; }

        /// <summary>
        /// Type of the media content.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Title of the media content.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The content rating of the media content.
        /// </summary>
        public string ContentRating { get; set; }

        /// <summary>
        /// Summary of the media content.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Index used for TV series
        /// </summary>
        public int Index { get; set; }
        
        /// <summary>
        /// Rating of the media content.
        /// </summary>
        public double Rating { get; set; }
        
        /// <summary>
        /// Audience rating of the media content used for movies.
        /// </summary>
        public double AudienceRating { get; set; }

        /// <summary>
        /// Count of times viewed.
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// Last viewed time in epochs.
        /// </summary>
        public int LastViewedAt { get; set; }

        /// <summary>
        /// The year the media was produced in.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Short punchline of the media used for movies.
        /// </summary>
        public string Tagline { get; set; }
        
        /// <summary>
        /// The thumbnail for the media.
        /// </summary>
        public string Thumb { get; set; }
        
        /// <summary>
        /// The poster for the media.
        /// </summary>
        public string Art { get; set; }
        
        /// <summary>
        /// The banner for the media used for TV Series.
        /// </summary>
        public string Banner { get; set; }

        /// <summary>
        /// Duration in milliseconds.
        /// </summary>
        public int Duration { get; set; }
        
        /// <summary>
        /// Date of original launch.
        /// </summary>
        public DateTime OriginallyAvailableAt { get; set; }
        
        /// <summary>
        /// Count of seasons of the TV Series.
        /// </summary>
        public int ChildCount { get; set; }
        
        /// <summary>
        /// Updated lastly in epoch time.
        /// </summary>
        public int UpdatedAt { get; set; }
        
        /// <summary>
        /// Added at in epoch time.
        /// </summary>
        public int AddedAt { get; set; }

        /// <summary>
        /// Audience rating image url.
        /// </summary>
        public string AudienceRatingImage { get; set; }

        /// <summary>
        /// Chapter source of the media.
        /// </summary>
        public string ChapterSource { get; set; }

        /// <summary>
        /// Wether or not the media has premium features.
        /// </summary>
        public string HasPremiumExtras { get; set; }
        
        /// <summary>
        /// Wether or not the media has premium feature extras.
        /// </summary>
        public string HasPremiumPrimaryExtra { get; set; }
        
        /// <summary>
        /// Rating image url.
        /// </summary>
        public string RatingImage { get; set; }

        /// <summary>
        /// Collection of media content used for movies.
        /// </summary>
        public IEnumerable<Media> Media { get; set; } = new List<Media>();
        
        /// <summary>
        /// Collection of media genres.
        /// </summary>
        public IEnumerable<TagWithValue> Genre { get; set; } = new List<TagWithValue>();
        
        /// <summary>
        /// Collection of media directors used for movies.
        /// </summary>
        public IEnumerable<TagWithValue> Director { get; set; } = new List<TagWithValue>();
        
        /// <summary>
        /// Collection of media writer used for movies.
        /// </summary>
        public IEnumerable<TagWithValue> Writer { get; set; } = new List<TagWithValue>();
        
        /// <summary>
        /// Collection of media countries used for movies.
        /// </summary>
        public IEnumerable<TagWithValue> Country { get; set; } = new List<TagWithValue>();
        
        /// <summary>
        /// Collection of media roles | cast.
        /// </summary>
        public IEnumerable<TagWithValue> Role { get; set; } = new List<TagWithValue>();
    }
}