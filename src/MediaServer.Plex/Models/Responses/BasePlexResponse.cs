using MediaServer.Plex.Models.Config;

namespace MediaServer.Plex.Models.Responses
{
    /// <summary>
    /// Response model for server preferences.
    /// </summary>
    public class BasePlexResponse<T>
    {
        public T MediaContainer { get; set; }
    }
}