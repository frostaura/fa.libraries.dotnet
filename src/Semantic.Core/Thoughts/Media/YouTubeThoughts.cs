using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.SkillDefinition;
using System.ComponentModel;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.Media
{
    /// <summary>
    /// YouTube media thoughts.
    /// </summary>
    public class YouTubeThoughts : BaseThought
    {
        /// <summary>
        /// HTTP client factory.
        /// </summary>
        private readonly IHttpClientFactory _httpClientFactory;
        /// <summary>
        /// The Google API client.
        /// </summary>
        private readonly GoogleConfig _googleConfig;

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="httpClientFactory">HTTP client factory.</param>
        /// <param name="googleConfig">The Pexels SDK client.</param>
        /// <param name="logger">Instance logger.</param>
        public YouTubeThoughts(IHttpClientFactory httpClientFactory, IOptions<GoogleConfig> googleConfig, ILogger<YouTubeThoughts> logger)
            :base(logger)
        {
            _httpClientFactory = httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
            _googleConfig = googleConfig
                .ThrowIfNull(nameof(googleConfig))
                .Value
                .ThrowIfNull(nameof(googleConfig));
        }

        /// <summary>
        /// Publish a local video file to YouTube.
        /// </summary>
        /// <param name="filePath">The file path to the local video.</param>
        /// <param name="description">The description of the video.</param>
        /// <param name="tags">The space-delimited tags for the video.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>OK when successful.</returns>
        [SKFunction, Description("Publish a local video file to YouTube.")]
        public async Task<string> PublishLocalVideoToYouTubeAsync(
            [Description("The file path to the local video.")] string filePath,
            [Description("The description of the video.")] string description,
            [Description("The space-delimited tags for the video.")] string tags,
            CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}
