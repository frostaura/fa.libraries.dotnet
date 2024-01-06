using ElevenLabs;
using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive
{
	/// <summary>
	/// Audio thoughts.
	/// </summary>
	public class AudioThoughts : BaseThought
    {
        /// <summary>
        /// ElevenLabs API client.
        /// </summary>
        private readonly ElevenLabsClient _elevenLabsClient;

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="elevenLabsConfig">ElevenLabs configuration.</param>
        /// <param name="logger">Instance logger.</param>
        public AudioThoughts(IOptions<ElevenLabsConfig> elevenLabsConfig, ILogger<AudioThoughts> logger)
            :base(logger)
        {
            var apiKey = elevenLabsConfig
                .ThrowIfNull(nameof(elevenLabsConfig))
                .Value
                .ThrowIfNull(nameof(elevenLabsConfig))
                .ApiKey
                .ThrowIfNullOrWhitespace(nameof(elevenLabsConfig.Value.ApiKey));
            _elevenLabsClient = new ElevenLabsClient(apiKey);
        }

        /// <summary>
        /// Convert text to speech and returns the local file path to the audio file.
        /// </summary>
        /// <param name="text">The text to speak.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>A local file path to the Audio file.</returns>
        [KernelFunction, Description("Convert text to speech and returns the local file path to the audio file.")]
        public async Task<string> TextToSpeechAsync(
            [Description("The text to speak.")] string text,
            CancellationToken token = default)
        {
            var voices = await _elevenLabsClient
                .VoicesEndpoint
                .GetAllVoicesAsync();
            var defaultVoiceSettings = await _elevenLabsClient
                .VoicesEndpoint
                .GetDefaultVoiceSettingsAsync();
            var clip = await _elevenLabsClient
                .TextToSpeechEndpoint
                .TextToSpeechAsync(text.ThrowIfNullOrWhitespace(nameof(text)), voices.FirstOrDefault(), defaultVoiceSettings);
            string clipPath = default;

            throw new NotImplementedException("Save clip first as old API used to do.");

            return clipPath;
        }
    }
}
