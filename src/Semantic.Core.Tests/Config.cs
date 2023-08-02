using FrostAura.Libraries.Semantic.Core.Models.Configuration;

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
            "Create me an end-to-end YouTube short video on a currently trending topic, ai-generating any video and audio requirements and finally auto-uploading it to my YouTube channel."
        };

		/// <summary>
		/// Universal semantic config.
		/// </summary>
		public static SemanticConfig SEMANTIC_CONFIG => new SemanticConfig
		{
			OpenAIConfig = new OpenAIConfig
            {
                Endpoint = "https://der-aic-devtest-openai-labs-eus.openai.azure.com/",
                ApiKey = "53bf2c61bbf543fc8ec9f149373dbaa1",
                LargeModelName = "gpt-4-32k",
                EmbeddingModelName = "text-embedding-ada-002",
                SmallModelName = "gpt-35-turbo-16k"
            },
			PineconeConfig = new PineconeConfig
			{
				ApiKey = "aad6ee60-dd2e-4386-9732-409b45035e3b",
				Environment = "us-east-1-aws"
            }
        };
	}
}

