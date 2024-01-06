using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Semantic.Core.Tests.Thoughts.Cognitive
{
    public class AudioThoughtsTests
    {
		[Fact]
		public void Constructor_WithInvalidConfig_ShouldThrow()
		{
            IOptions<ElevenLabsConfig> elevenLabsConfig = default;
            ILogger<AudioThoughts> logger = Substitute.For<ILogger<AudioThoughts>>();

            var actual = Assert.Throws<ArgumentNullException>(() => new AudioThoughts(elevenLabsConfig, logger));

            Assert.Equal(nameof(elevenLabsConfig), actual.ParamName);
		}

        [Fact]
        public void Constructor_WithInvalidLogger_ShouldThrow()
        {
            IOptions<ElevenLabsConfig> elevenLabsConfig = Substitute.For<IOptions<ElevenLabsConfig>>();
            elevenLabsConfig.Value.Returns(Config.SEMANTIC_CONFIG.ElevenLabsConfig);
            ILogger<AudioThoughts> logger = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new AudioThoughts(elevenLabsConfig, logger));

            Assert.Equal(nameof(logger), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            var elevenLabsConfig = Substitute.For<IOptions<ElevenLabsConfig>>();
            elevenLabsConfig.Value.Returns(Config.SEMANTIC_CONFIG.ElevenLabsConfig);
            var logger = Substitute.For<ILogger<AudioThoughts>>();

            var actual = new AudioThoughts(elevenLabsConfig, logger);

            Assert.NotNull(actual);
        }

        [Fact]
        public async Task TextToSpeechAsync_WithInvalidText_ShouldThrow()
        {
            var elevenLabsConfig = Substitute.For<IOptions<ElevenLabsConfig>>();
            elevenLabsConfig.Value.Returns(Config.SEMANTIC_CONFIG.ElevenLabsConfig);
            var logger = Substitute.For<ILogger<AudioThoughts>>();
            var instance = new AudioThoughts(elevenLabsConfig, logger);
            string text = default;

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.TextToSpeechAsync(text));

            Assert.Equal(nameof(text), actual.ParamName);
        }

        [Fact(Skip = "Integration Test")]
        public async Task TextToSpeechAsync_WithValidText_ShouldReturnFilePath()
        {
            var elevenLabsConfig = Substitute.For<IOptions<ElevenLabsConfig>>();
            elevenLabsConfig.Value.Returns(Config.SEMANTIC_CONFIG.ElevenLabsConfig);
            var logger = Substitute.For<ILogger<AudioThoughts>>();
            var instance = new AudioThoughts(elevenLabsConfig, logger);
            string text = "Hi, my name is Iluvatar. How are you?";

            var actual = await instance.TextToSpeechAsync(text);

            Assert.NotEmpty(actual);
            Assert.Contains(".mp3", actual);
        }
    }
}
