using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;
using System.Net.Http.Headers;
using FrostAura.Libraries.Semantic.Core.Models.ImgBB;
using Newtonsoft.Json;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.Media;

/// <summary>
/// Media storage thoughts.
/// </summary>
public class MediaStorageThoughts : BaseThought
{
    /// <summary>
    /// HTTP client factory.
    /// </summary>
    private readonly IHttpClientFactory _httpClientFactory;
    /// <summary>
    /// Configuration for IMGBB.
    /// </summary>
    private readonly ImgbbConfig _imgbbConfig;

    /// <summary>
    /// Overloaded constructor to provide dependencies.
    /// </summary>
    /// <param name="serviceProvider">The dependency service provider.</param>
    /// <param name="semanticKernelLanguageModels">A component for communicating with language models.</param>
    /// <param name="httpClientFactory">HTTP client factory.</param>
    /// <param name="imgbbConfig">Configuration for IMGBB.</param>
    /// <param name="logger">Instance logger.</param>
    public MediaStorageThoughts(IServiceProvider serviceProvider, ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels, IHttpClientFactory httpClientFactory, IOptions<ImgbbConfig> imgbbConfig, ILogger<MediaStorageThoughts> logger)
        :base(serviceProvider, semanticKernelLanguageModels, logger)
    {
        imgbbConfig.ThrowIfNull(nameof(imgbbConfig));

        _httpClientFactory = httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
        _imgbbConfig = imgbbConfig
            .Value
            .ThrowIfNull(nameof(imgbbConfig));
    }

    /// <summary>
    /// Upload a local image file to the cloud and returns the full URL of the now-hosted image.
    /// </summary>
    /// <param name="filePath">The local image file path.</param>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The full URL of the now-hosted image.</returns>
    [KernelFunction, Description("Upload a local image file to the cloud and returns the full URL of the now-hosted image. This can be used to host images if URLs for local images are required, for example.")]
    public async Task<string> UploadImageFromFileAndGetUrlAsync(
        [Description("The local image file path.")] string filePath,
        CancellationToken token = default)
    {
        using (_logger.BeginScope("{MethodName}", nameof(UploadImageFromFileAndGetUrlAsync)))
        {
            _logger.LogInformation("Uploading local image {FilePath}.", filePath);

            using (var httpClient = _httpClientFactory.CreateClient())
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(_imgbbConfig.ApiKey.ThrowIfNullOrWhitespace(nameof(_imgbbConfig.ApiKey))), "key");

                var imageBytes = File.ReadAllBytes(filePath);
                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

                formData.Add(imageContent, "image", Path.GetFileName(filePath));

                using (var response = await httpClient.PostAsync("https://api.imgbb.com/1/upload", formData, token))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Failed to upload image: {response.ReasonPhrase}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync(token);
                    var parsedResponse = JsonConvert.DeserializeObject<ImgBBResponse>(responseContent);

                    return parsedResponse.Data.Url;
                }
            }
        }
    }
}
