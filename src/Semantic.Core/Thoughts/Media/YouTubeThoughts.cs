using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.Media
{
    /// <summary>
    /// YouTube media thoughts.
    /// </summary>
    public class YouTubeThoughts : BaseThought
    {
        /// <summary>
        /// The Google API client.
        /// </summary>
        private readonly GoogleConfig _googleConfig;
        /// <summary>
        /// YouTube service.
        /// </summary>
        private readonly YouTubeService _youtubeService;

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="serviceProvider">The dependency service provider.</param>
        /// <param name="semanticKernelLanguageModels">A component for communicating with language models.</param>
        /// <param name="googleConfig">The Pexels SDK client.</param>
        /// <param name="logger">Instance logger.</param>
        public YouTubeThoughts(IServiceProvider serviceProvider, ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels, IOptions<GoogleConfig> googleConfig, ILogger<YouTubeThoughts> logger)
            :base(serviceProvider, semanticKernelLanguageModels, logger)
        {
            _googleConfig = googleConfig
                .ThrowIfNull(nameof(googleConfig))
                .Value
                .ThrowIfNull(nameof(googleConfig));
            _youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = _googleConfig.OAuthToken
            });
        }

        /// <summary>
        /// Publish a local video file to YouTube.
        /// </summary>
        /// <param name="filePath">The file path to the local video.</param>
        /// <param name="title">The title of the video.</param>
        /// <param name="categoryId">The YouTube API categoryId as a stringified integer. Example: '23' (Comedy), '15' (Pets & Animals)</param>
        /// <param name="description">The description of the video.</param>
        /// <param name="tags">The space-delimited tags for the video.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>OK when successful.</returns>
        [KernelFunction, Description("Publish a local video file to YouTube.")]
        public async Task<string> PublishLocalVideoToYouTubeAsync(
            [Description("The file path to the local video.")] string filePath,
            [Description("The title of the video.")] string title,
            [Description("The YouTube API categoryId as a stringified integer. Example: '23' (Comedy), '15' (Pets & Animals)")] string categoryId,
            [Description("The description of the video.")] string description,
            [Description("The space-delimited tags for the video.")] string tags,
            CancellationToken token = default)
        {
            // Create a video object with metadata.
            var video = new Video
            {
                Snippet = new VideoSnippet
                {
                    Title = title.ThrowIfNullOrWhitespace(nameof(title)),
                    Description = description.ThrowIfNullOrWhitespace(nameof(description)),
                    Tags = tags.ThrowIfNullOrWhitespace(nameof(tags)).Split(" "),
                    CategoryId = $"{int.Parse(categoryId.ThrowIfNullOrWhitespace(nameof(categoryId)))}"
                },
                Status = new VideoStatus
                {
                    PrivacyStatus = "public"
                }
            };

            // Create a video upload request.
            var insertRequest = _youtubeService
                .Videos
                .Insert(video, "snippet,status", File.OpenRead(filePath.ThrowIfNullOrWhitespace(nameof(filePath))), "video/*");

            insertRequest.ProgressChanged += (progress) =>
            {
                // Handle upload progress.
                switch (progress.Status)
                {
                    case UploadStatus.Uploading:
                        LogSemanticInformation($"Uploading: {progress.BytesSent} bytes sent.");
                        break;
                    case UploadStatus.Completed:
                        LogSemanticInformation("Upload completed.");
                        break;
                    case UploadStatus.Failed:
                        LogSemanticError($"Upload failed: {progress.Exception}", progress.Exception);
                        break;
                }
            };

            // Execute the upload request.
            await insertRequest.UploadAsync(token);

            var uploadedVideo = insertRequest.ResponseBody;

            return $"Video uploaded successfully! Video ID: {uploadedVideo.Id}";
        }
    }
}
