using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FrostAura.Libraries.Core.Extensions.Decoration;
using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Http.Extensions;
using FrostAura.Libraries.Http.Interfaces;
using FrostAura.Libraries.Http.Models.Requests;
using FrostAura.Libraries.Http.Models.Responses;
using MediaServer.Core.Enums;
using MediaServer.Core.Interfaces;
using MediaServer.Core.Models.Content;
using MediaServer.Plex.Enums;
using MediaServer.Plex.Extensions;
using MediaServer.Plex.Models.Collections;
using MediaServer.Plex.Models.Config;
using MediaServer.Plex.Models.Content;
using MediaServer.Plex.Models.Responses;

namespace MediaServer.Plex.Services
{
    /// <summary>
    /// Plex Media Server service.
    /// </summary>
    public class PlexMediaService : IMediaServer<PlexMediaServerConfig>
    {
        /// <summary>
        /// Media server specific configuration. Should be set in a constructor.
        /// </summary>
        public PlexMediaServerConfig Configuration { get; }

        /// <summary>
        /// Instance of static http service to use in making web requests.
        /// </summary>
        private IHttpService _httpService { get; }

        /// <summary>
        /// Overloaded constructor to pass configuration.
        /// </summary>
        /// <param name="configuration">Media server specific configuration.</param>
        /// <param name="httpService">Instance of static http service to use in making web requests.</param>
        public PlexMediaService(PlexMediaServerConfig configuration, IHttpService httpService)
        {
            Configuration = configuration
                .ThrowIfNull(nameof(configuration))
                .ThrowIfInvalid(nameof(configuration));

            _httpService = httpService
                .ThrowIfNull(nameof(httpService));
        }

        /// <summary>
        /// Media server initialized to be called before consuming the service.
        /// <param name="token">Cancellation token instance.</param>
        /// </summary>
        public async Task InitializeAsync(CancellationToken token)
        {
            Configuration.ServerPreferences = await GetServerPreferencesAsync(token);
        }

        /// <summary>
        /// Collection of libraries from the server
        /// <param name="token">Cancellation token instance.</param>
        /// </summary>
        public async Task<IEnumerable<Library>> GetAllLibrariesAsync(CancellationToken token)
        {
            var requestUrl = Endpoint.Libraries.Description(Configuration.ServerAddress);
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            HttpRequest httpRequest = request
                .WithAuthToken(Configuration)
                .AcceptJson()
                .ToHttpRequest();
            HttpResponse<BasePlexResponse<Libraries>> response = await _httpService
                .RequestAsync<BasePlexResponse<Libraries>>(httpRequest, token);
            List<Library> result = response
                .Response
                .MediaContainer
                .Directory
                .Select(d => new Library
                {
                    Id = d.Key,
                    Poster = $"{Configuration.ServerAddress}{d.Art}?{Configuration.QueryStringPlexToken}",
                    Thumbnail = $"{Configuration.ServerAddress}{d.Thumb}?{Configuration.QueryStringPlexToken}",
                    Title = d.Title,
                    Type = GetTypeFromString(d.Type),
                    GetMoviesAsync = GetMoviesAsync(d.Key, d.Type, token)
                })
                .ToList();

            return result;
        }

        /// <summary>
        /// Get Plex server preferences from an HTTP endpoint.
        /// <param name="token">Cancellation token instance.</param>
        /// </summary>
        private async Task<ServerPreferences> GetServerPreferencesAsync(CancellationToken token)
        {
            var requestUrl = Endpoint.ServerPreferences.Description(Configuration.ServerAddress);
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            HttpRequest httpRequest = request
                .WithAuthToken(Configuration)
                .AcceptJson()
                .ToHttpRequest();
            HttpResponse<BasePlexResponse<ServerPreferences>> response = await _httpService
                .RequestAsync<BasePlexResponse<ServerPreferences>>(httpRequest, token);

            return response.Response?.MediaContainer;
        }
        
        /// <summary>
        /// Convert the string of library type to an enum value.
        /// </summary>
        /// <param name="str">String library type.</param>
        /// <returns>Enum library type.</returns>
        private LibraryType GetTypeFromString(string str)
        {
            switch (str)
            {
                case "movie":
                    return LibraryType.Movie;
                case "show":
                    return LibraryType.TvSeries;
                case "artist":
                    return LibraryType.Music;
                default:
                    return LibraryType.Other;
            }
        }

        /// <summary>
        /// Get all movies async.
        /// </summary>
        /// <param name="libraryId">The ID of the library to get the content for.</param>
        /// <param name="libraryType">The string type for the library.</param>
        /// <param name="token">Cancellation token instance.</param>
        /// <returns>Movies collection</returns>
        private async Task<IEnumerable<Movie>> GetMoviesAsync(string libraryId, string libraryType, CancellationToken token)
        {
            LibraryType type = GetTypeFromString(libraryType.ThrowIfNullOrWhitespace(nameof(libraryType)));
            var movies = new List<Movie>();

            if (type != LibraryType.Movie) return movies;
            
            var requestUrl = Endpoint.LibraryMovies.Description(Configuration.ServerAddress, libraryId.ThrowIfNullOrWhitespace(nameof(libraryId)));
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            HttpRequest httpRequest = request
                .WithAuthToken(Configuration)
                .AcceptJson()
                .ToHttpRequest();
            HttpResponse<BasePlexResponse<MediaContainer>> response = await _httpService
                .RequestAsync<BasePlexResponse<MediaContainer>>(httpRequest, token);

            movies = response
                .Response
                .MediaContainer
                .Metadata
                .Select(m =>
                {
                    Media media = m.Media.First();
                    
                    return new Movie
                    {
                        AudioChannels = media.AudioChannels,
                        AudioCodec = media.AudioCodec,
                        Bitrate = media.Bitrate,
                        Container = media.Container,
                        Description = m.Summary,
                        Duration = m.Duration,
                        Height = media.Height,
                        Width = media.Width,
                        Poster = $"{Configuration.ServerAddress}{m.Art}?{Configuration.QueryStringPlexToken}",
                        Rating = m.Rating,
                        StreamingUrl = $"{Configuration.ServerAddress}{media.Part.First().Key}?{Configuration.QueryStringPlexToken}",
                        Studio = m.Studio,
                        Thumbnail = $"{Configuration.ServerAddress}{m.Thumb}?{Configuration.QueryStringPlexToken}",
                        Title = m.Title,
                        VideoCodec = media.VideoCodec,
                        ViewCount = m.ViewCount,
                        Year = m.Year
                    };
                })
                .ToList();

            return movies;
        }
    }
}