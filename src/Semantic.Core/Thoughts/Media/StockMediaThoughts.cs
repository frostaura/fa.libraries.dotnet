using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.SkillDefinition;
using PexelsDotNetSDK.Api;
using System;
using System.ComponentModel;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.Media
{
    /// <summary>
    /// Stock media thoughts.
    /// </summary>
    public class StockMediaThoughts : BaseThought
    {
        /// <summary>
        /// HTTP client factory.
        /// </summary>
        private readonly IHttpClientFactory _httpClientFactory;
        /// <summary>
        /// The Pexels SDK client.
        /// </summary>
        private readonly PexelsClient _pexelsClient;

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="httpClientFactory">HTTP client factory.</param>
        /// <param name="pexelsConfig">The Pexels SDK client.</param>
        /// <param name="logger">Instance logger.</param>
        public StockMediaThoughts(IHttpClientFactory httpClientFactory, IOptions<PexelsConfig> pexelsConfig, ILogger<StockMediaThoughts> logger)
            :base(logger)
        {
            pexelsConfig.ThrowIfNull(nameof(pexelsConfig));

            _httpClientFactory = httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
            _pexelsClient = new PexelsClient(pexelsConfig
                .Value
                .ThrowIfNull(nameof(pexelsConfig))
                .ApiKey
                .ThrowIfNullOrWhitespace(nameof(pexelsConfig.Value.ApiKey)));
        }

        /// <summary>
        /// Get a local file path to a stock video matching a search phrase.
        /// </summary>
        /// <param name="searchQuery">The video search term.</param>
        /// <param name="orientation">The orientation of the video.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>A collection of local file paths for the videos downloaded.</returns>
        [SKFunction, Description("Get local file paths to stock videos matching a search phrase.")]
        public async Task<string> DownloadAndGetStockVideoAsync(
            [Description("The video search term.")] string searchQuery,
            [Description("The video orientation (portrait|landscape|square|all).")] string orientation = "all",
            CancellationToken token = default)
        {
            var result = await _pexelsClient.SearchVideosAsync(
                searchQuery.ThrowIfNullOrWhitespace(nameof(searchQuery)),
                orientation: orientation.Replace("all", string.Empty));
            var video = result
                .videos
                .Where(v => v.videoFiles.Any(v => v.quality == "hd"))
                .Select(v => v.videoFiles.First(vv => vv.quality == "hd"))
                .First();
            var filePath = $"videos/{Guid.NewGuid().ToString().Replace("-", "")}.mp4";

            if (!Directory.Exists("videos")) Directory.CreateDirectory("videos");

            await DownloadVideoUrlToFileAsync(video.link, filePath, token);

            return filePath;
        }

        /// <summary>
        /// Download a video from a UR to a file.
        /// </summary>
        /// <param name="videoUrl">The video link to save.</param>
        /// <param name="savePath">The path to save it to.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>Void</returns>
        private async Task DownloadVideoUrlToFileAsync(string videoUrl, string savePath, CancellationToken token)
        {
            using (var client = _httpClientFactory.CreateClient())
            {
                using (var response = await client.GetAsync(videoUrl, token))
                {
                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                                  stream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        await contentStream.CopyToAsync(stream);
                    }
                }
            }
        }
    }
}
