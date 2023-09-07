using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using FrostAura.Libraries.Semantic.Core.Tests;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Semantic.Core.Tests
{
    /// <summary>
    /// Static testing configuration.
    /// </summary>
    public static class Config
	{
		/// <summary>
		/// Use cases that should be supported during the progression of the project.
		/// </summary>
		public static IReadOnlyList<string> CORE_USE_CASES = new List<string>
		{
            "Summarize https://www.imdb.com/chart/moviemeter/ for me.",
            "Write me a book about a currently trending topic, including ai-generating the cover art and finally self-publishing it to Amazon.",
            "Create me an end-to-end YouTube short video on a currently trending topic, ai-generating any video and Memory requirements and finally auto-uploading it to my YouTube channel."
        };

		/// <summary>
		/// Universal semantic config.
		/// </summary>
		public static SemanticConfig SEMANTIC_CONFIG => new SemanticConfig
		{
			OpenAIConfig = new OpenAIConfig
            {
                Endpoint = string.Empty,
                ApiKey = string.Empty,
                LargeModelName = string.Empty,
                EmbeddingModelName = string.Empty,
                SmallModelName = string.Empty
            },
			PineconeConfig = new PineconeConfig
			{
				ApiKey = string.Empty,
				Environment = string.Empty
            },
			PexelsConfig = new PexelsConfig
			{
				ApiKey = string.Empty
            },
            ElevenLabsConfig = new ElevenLabsConfig
			{
                ApiKey = string.Empty
            },
			GoogleConfig = new GoogleConfig
			{
				ApiKey = string.Empty
			}
        };

		/// <summary>
		/// Create an instance of a real logger when required.
		/// </summary>
		/// <typeparam name="T">The type of the object the logger is for.</typeparam>
		/// <returns>The logger instance.</returns>
		public static ILogger<T> GetLogger<T>(ITestOutputHelper testOutputHelper)
		{
            var loggerFactory = LoggerFactory.Create(l =>
            {
                l.AddProvider(new XunitLoggerProvider(testOutputHelper));
            });

            return loggerFactory.CreateLogger<T>();
        }
	}
}

