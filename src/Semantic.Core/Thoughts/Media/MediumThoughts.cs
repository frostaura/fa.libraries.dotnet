using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using FrostAura.Libraries.Semantic.Core.Models.Medium;
using System.Text;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.Media;

/// <summary>
/// Medium blogging platform thoughts.
/// </summary>
public class MediumThoughts : BaseThought
{
    /// <summary>
    /// HTTP client factory.
    /// </summary>
    private readonly IHttpClientFactory _httpClientFactory;
    /// <summary>
    /// Medium platform config.
    /// </summary>
    private readonly MediumConfig _mediumConfig;

    /// <summary>
    /// Overloaded constructor to provide dependencies.
    /// </summary>
    /// <param name="serviceProvider">The dependency service provider.</param>
    /// <param name="semanticKernelLanguageModels">A component for communicating with language models.</param>
    /// <param name="httpClientFactory">HTTP client factory.</param>
    /// <param name="mediumConfig">The mediu SDK client.</param>
    /// <param name="logger">Instance logger.</param>
    public MediumThoughts(IServiceProvider serviceProvider, ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels, IHttpClientFactory httpClientFactory, IOptions<MediumConfig> mediumConfig, ILogger<MediumThoughts> logger)
        :base(serviceProvider, semanticKernelLanguageModels, logger)
    {
        _mediumConfig = mediumConfig
            .ThrowIfNull(nameof(mediumConfig))
            .Value
            .ThrowIfNull(nameof(mediumConfig));
        _httpClientFactory = httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
    }

    [KernelFunction, Description("Post a new Medium blog with HTML content.")]
    public async Task<string> PostMediumBlogHTMLAsync(
        [Description("The blog's title.")] string title,
        [Description(@"The HTML blog content, excluding any poster figure at the top. Medium only supports the following HTML tags. Images that are referenced should be absolute URLS and not local file names.
        --- Supported HTML Tags ---
        <h1>Heading 1</h1>
        <h2>Heading 2</h2>
        <p>Paragraph text</p>
        <blockquote>I am a blockquote</blockquote>
        <figure>
          <img src=""https://my-example-app/bey.png"">
          <figcaption>I am an optional caption</figcaption>
        </figure>

        <b>bold text</b>, <strong>also bold text</strong>
        <i>italic text</i>, <em>also italic text</em>
        <a href=""medium.com"">link here</a>
        // a section break
        <hr>
        ")] string content,
        [Description("Tags for the blog. These tags should be in a string array format. About 5 tags are usually enough. Example: ['tag-1', 'tag-2']")] string tags,
        [Description("The status which to post the article with. Available options are: 'draft', 'public'")] string publicationStatus = "draft",
        CancellationToken token = default)
    {
        using (_logger.BeginScope("{MethodName}", nameof(PostMediumBlogHTMLAsync)))
        {
            _logger.LogInformation("Posting a new blog to Medium with the title {Title} (Tags) as {PublicationStatus}.", title, tags, publicationStatus);

            using (var client = _httpClientFactory.CreateClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mediumConfig.Token.ThrowIfNullOrWhitespace(nameof(_mediumConfig.Token)));

                using (var meRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.medium.com/v1/me"))
                {
                    _logger.LogDebug("Generating a poster image.");

                    var llmThoughts = (LanguageModelThoughts)_serviceProvider.GetThoughtByName(nameof(LanguageModelThoughts));
                    var dallEPrompt = await llmThoughts.PromptLLMAsync("You are the world's best prompt engineer for the Dall-E 3 text to image model. You take a title for a blog and transform it to a creative but relevant to the content prompt that can be used to generate an image." +
                        $"Title: {title.ThrowIfNullOrWhitespace(nameof(title))}" +
                        "Dall-E 3 Prompt: ", token);
                    var dallEImageUrl = await llmThoughts.GenerateImageAndGetUrlAsync(dallEPrompt, token: token);
                    var contentHeader = $@"
                    <figure>
                      <img src=""{dallEImageUrl}"" alt=""{dallEPrompt}"">
                      <figcaption>Photo by Dall-E 3 (https://bing.com/create).</figcaption>
                    </figure>
                    <hr>
                    ";

                    _logger.LogDebug("Getting Medium user information.");

                    var response = await client.SendAsync(meRequest);
                    var responseStr = await response.Content.ReadAsStringAsync();
                    var me = JsonConvert.DeserializeObject<MeResponse>(responseStr);
                    var tagsParsed = tags
                        .ThrowIfNullOrWhitespace(nameof(tags))
                        .Replace("[", "")
                        .Replace("]", "")
                        .Replace("\"", "")
                        .Split(',');

                    _logger.LogDebug("Posting to Medium as '{PublicationStatus}'.", publicationStatus);

                    using (var postRequest = new HttpRequestMessage(HttpMethod.Post, $"https://api.medium.com/v1/users/{me.Data.Id}/posts"))
                    {
                        postRequest.Content = new StringContent(JsonConvert.SerializeObject(new
                        {
                            title,
                            contentFormat = "html",
                            content = contentHeader + content.ThrowIfNullOrWhitespace(nameof(content)),
                            tags = tagsParsed,
                            publishStatus = publicationStatus
                        }), Encoding.UTF8, "application/json");
                        var postResponse = await client.SendAsync(postRequest);
                        var postResponseStr = await postResponse.Content.ReadAsStringAsync();

                        _logger.LogDebug("Post to Medium Response: {PostResponseStr}", postResponseStr);
                        _logger.LogInformation("Posting successful.");
                        return postResponseStr;
                    }
                }
            }
        }
    }
}
