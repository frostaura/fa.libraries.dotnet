using FrostAura.Libraries.Semantic.Core.Enumerations.Semantic;
using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using Microsoft.SemanticKernel;

namespace FrostAura.Libraries.Semantic.Core.Extensions.Configuration
{
    /// <summary>
    /// OpenAI configuration extensions.
    /// </summary>
    public static class SemanticConfigExtensions
    {
		/// <summary>
		/// Build a semantic kernel from OpenAI config.
		/// </summary>
		/// <param name="config">OpenAI configuration.</param>
		/// <returns>A build semantic kernel.</returns>
		public static IKernel GetComprehensiveKernel(this SemanticConfig config)
		{
            var kernelConfig = new KernelConfig();
            kernelConfig.DefaultHttpRetryConfig.MaxRetryCount = 5;
            kernelConfig.DefaultHttpRetryConfig.UseExponentialBackoff = true;
            var kernelBuilder = new KernelBuilder()
                .WithConfiguration(kernelConfig)
                .WithPineconeMemoryStore(config.PineconeConfig.Environment, config.PineconeConfig.ApiKey);

            // Configure the public kernel.
            if (string.IsNullOrWhiteSpace(config.OpenAIConfig.Endpoint))
            {
                kernelBuilder
                    .WithOpenAIChatCompletionService(config.OpenAIConfig.LargeModelName, config.OpenAIConfig.ApiKey, orgId: config.OpenAIConfig.OrgId, serviceId: ModelType.LargeLLM.ToString(), setAsDefault: true)
                    .WithOpenAIChatCompletionService(config.OpenAIConfig.SmallModelName, config.OpenAIConfig.ApiKey, orgId: config.OpenAIConfig.OrgId, serviceId: ModelType.SmallLLM.ToString())
                    .WithOpenAITextEmbeddingGenerationService(config.OpenAIConfig.EmbeddingModelName, config.OpenAIConfig.ApiKey, orgId: config.OpenAIConfig.OrgId, serviceId: ModelType.Embedding.ToString())
                    .WithOpenAIImageGenerationService(config.OpenAIConfig.ApiKey, orgId: config.OpenAIConfig.OrgId, serviceId: ModelType.ImageGeneration.ToString());
            }
            // Configure the private kernel.
            else
            {
                kernelBuilder
                    .WithAzureChatCompletionService(config.OpenAIConfig.LargeModelName, config.OpenAIConfig.Endpoint, config.OpenAIConfig.ApiKey, serviceId: ModelType.LargeLLM.ToString(), setAsDefault: true)
                    .WithAzureChatCompletionService(config.OpenAIConfig.SmallModelName, config.OpenAIConfig.Endpoint, config.OpenAIConfig.ApiKey, serviceId: ModelType.SmallLLM.ToString())
                    .WithAzureTextEmbeddingGenerationService(config.OpenAIConfig.EmbeddingModelName, config.OpenAIConfig.Endpoint, config.OpenAIConfig.ApiKey, serviceId: ModelType.Embedding.ToString())
                    .WithAzureOpenAIImageGenerationService(config.OpenAIConfig.Endpoint, config.OpenAIConfig.ApiKey, serviceId: ModelType.ImageGeneration.ToString());
            }

            return kernelBuilder.Build();
        }
	}
}
