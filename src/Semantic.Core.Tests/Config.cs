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
                Endpoint = "https://exp-sc-001.openai.azure.com/",
                ApiKey = "18d271a0d1ca4e26bff547191473db20",
                LargeModelName = "gpt-4-32k",
                EmbeddingModelName = "text-embedding-ada-002",
                SmallModelName = "gpt-35-turbo-16k",
                TextToImageModelName = "Dalle3"
                // gpt-4-turbo-vision
            },
            PineconeConfig = new PineconeConfig
            {
                ApiKey = "aad6ee60-dd2e-4386-9732-409b45035e3b",
                Environment = "us-east-1-aws"
            },
            PexelsConfig = new PexelsConfig
            {
                ApiKey = "iTBjPu4DoFrw92PpTMR1yzSFT3ttuGEsOLWOT0B1EacF1VXKs0GGVbxT"
            },
            ElevenLabsConfig = new ElevenLabsConfig
            {
                ApiKey = "efbfadb7ce50701bf5899a515b0b5fd9"
            },
            GoogleConfig = new GoogleConfig
            {
                OAuthToken = ""
            },
            FNBConfig = new FNBConfig
            {
                Username = "dean.martin@frostaura.net",
                Password = "wunRo1-bofgot-hifsic"
            },
            SemanticMemoryConfig = new SemanticMemoryConfig
            {
                CollectionName = "frostaura-semantic-core"
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
