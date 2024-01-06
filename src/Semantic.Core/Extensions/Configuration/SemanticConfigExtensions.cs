using System.Net;
using FrostAura.Libraries.Semantic.Core.Enumerations.Semantic;
using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using PluginsMemory = Microsoft.SemanticKernel.Plugins.Memory;
using Polly;

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
        /// <param name="memory">Memory, if should use a pre-existing memory store.</param>
		/// <returns>A build semantic kernel.</returns>
		public static Kernel GetComprehensiveKernel(this SemanticConfig config, ISemanticTextMemory memory = null)
		{
            var builder = Kernel
                .CreateBuilder();
            var memoryBuilder = new MemoryBuilder()
                .WithMemoryStore(new PluginsMemory.VolatileMemoryStore());
            // TODO: .WithPineconeMemoryStore(config.PineconeConfig.Environment, config.PineconeConfig.ApiKey);

            builder
                .Services
                .AddLogging(c => c
                    .AddDebug()
                    .SetMinimumLevel(LogLevel.Trace))
                .ConfigureHttpClientDefaults(c =>
                {
                    // Use a standard resiliency policy
                    c.AddStandardResilienceHandler().Configure(o =>
                    {
                        o.Retry.BackoffType = DelayBackoffType.Exponential;
                        o.Retry.MaxRetryAttempts = 5;
                        o.Retry.UseJitter = true;
                        o.Retry.ShouldHandle = args => ValueTask.FromResult(args.Outcome.Result?.StatusCode is not HttpStatusCode.OK);
                    });
                });

            // TODO: Register all plugins here for out-of-the-box planners.
            /*Assembly
                .GetAssembly(typeof(BaseThought))
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(BaseThought)) && !t.IsAbstract)
                .ToList()
                .ForEach(t => builder.Plugins.AddFromType<>());*/

            // Configure the public kernel.
            if (string.IsNullOrWhiteSpace(config.OpenAIConfig.Endpoint))
            {
                builder
                    .Services
                    .AddOpenAIChatCompletion(config.OpenAIConfig.LargeModelName, config.OpenAIConfig.ApiKey, serviceId: ModelType.LargeLLM.ToString(), orgId: config.OpenAIConfig.OrgId)
                    .AddOpenAIChatCompletion(config.OpenAIConfig.SmallModelName,  config.OpenAIConfig.ApiKey, serviceId: ModelType.SmallLLM.ToString(), orgId: config.OpenAIConfig.OrgId)
                    .AddOpenAITextEmbeddingGeneration(config.OpenAIConfig.EmbeddingModelName, config.OpenAIConfig.ApiKey, serviceId: ModelType.Embedding.ToString(), orgId: config.OpenAIConfig.OrgId)
                    .AddOpenAITextToImage(config.OpenAIConfig.ApiKey, serviceId: ModelType.ImageGeneration.ToString(), orgId: config.OpenAIConfig.OrgId);
                memoryBuilder
                    .WithOpenAITextEmbeddingGeneration(config.OpenAIConfig.EmbeddingModelName, config.OpenAIConfig.ApiKey, orgId: config.OpenAIConfig.OrgId);
            }
            // Configure the private kernel.
            else
            {
                builder
                    .Services
                    .AddAzureOpenAIChatCompletion(config.OpenAIConfig.LargeModelName, config.OpenAIConfig.Endpoint, config.OpenAIConfig.ApiKey, serviceId: ModelType.LargeLLM.ToString())
                    .AddAzureOpenAIChatCompletion(config.OpenAIConfig.SmallModelName, config.OpenAIConfig.Endpoint, config.OpenAIConfig.ApiKey, serviceId: ModelType.SmallLLM.ToString())
                    .AddAzureOpenAITextEmbeddingGeneration(config.OpenAIConfig.EmbeddingModelName, config.OpenAIConfig.Endpoint, config.OpenAIConfig.ApiKey, serviceId: ModelType.Embedding.ToString())
                    .AddAzureOpenAITextToImage(config.OpenAIConfig.TextToImageModelName, config.OpenAIConfig.Endpoint, config.OpenAIConfig.ApiKey, serviceId: ModelType.ImageGeneration.ToString());
                memoryBuilder
                    .WithAzureOpenAITextEmbeddingGeneration(config.OpenAIConfig.EmbeddingModelName, config.OpenAIConfig.Endpoint, config.OpenAIConfig.ApiKey);
            }

            builder
                .Services
                .AddSingleton<ISemanticTextMemory>(memory ?? memoryBuilder.Build());

            return builder.Build();
        }
	}
}
