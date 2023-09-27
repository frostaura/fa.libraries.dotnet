using System.Reflection;
using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using FrostAura.Libraries.Semantic.Core.Thoughts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace FrostAura.Libraries.Semantic.Core.Extensions.Configuration
{
	/// <summary>
	/// Service provider extensions.
	/// </summary>
	public static class ServiceProviderExtensions
	{
        /// <summary>
        /// Get a though instance from it's name.
        /// </summary>
        /// <param name="serviceProvider">The service provider for dependency injection.</param>
        /// <param name="name">Name of the action.</param>
        /// <returns>The action instance.</returns>
        public static BaseThought GetThoughtByName(this IServiceProvider serviceProvider, string name)
        {
            var allThoughts = serviceProvider
                .GetServices<BaseThought>();
            var action = allThoughts
                .FirstOrDefault(a => a.GetType().Name.Contains(name));

            return action;
        }

        /// <summary>
        /// Add Semantic Core services.
        /// </summary>
        /// <param name="services">Services collection to add to.</param>
        /// <param name="config">Configuration to use.</param>
        /// <returns>The ammended collection.</returns>
        public static IServiceCollection AddSemanticCore(this IServiceCollection services, SemanticConfig config)
		{
			return services
				.AddAllThoughts()
                .AddSemanticServices(config)
                .AddLogging()
                .AddHttpClient();
		}

        /// <summary>
        /// Add all thoughts as services.
        /// </summary>
        /// <param name="services">Services collection to add to.</param>
        /// <returns>The ammended collection.</returns>
        private static IServiceCollection AddAllThoughts(this IServiceCollection services)
        {
            Assembly
                .GetAssembly(typeof(BaseThought))
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(BaseThought)) && !t.IsAbstract)
                .ToList()
                .ForEach(t => services.AddTransient(typeof(BaseThought), t));

            return services;
        }

        /// <summary>
        /// Add all Semantic Kernel dependency services.
        /// </summary>
        /// <param name="services">Services collection to add to.</param>
        /// <param name="config">Configuration to use.</param>
        /// <returns>The ammended collection.</returns>
        private static IServiceCollection AddSemanticServices(this IServiceCollection services, SemanticConfig config)
        {
            var semanticConfig = config.GetComprehensiveKernel();

            return services
                .AddSingleton(Options.Create(config.ElevenLabsConfig))
                .AddSingleton(Options.Create(config.OpenAIConfig))
                .AddSingleton(Options.Create(config.PexelsConfig))
                .AddSingleton(Options.Create(config.PineconeConfig))
                .AddSingleton(Options.Create(config.FNBConfig))
                .AddTransient(typeof(IKernel), provider => semanticConfig);
        }
    }
}
