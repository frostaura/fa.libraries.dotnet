using System.Reflection;
using FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;
using FrostAura.Libraries.Semantic.Core.Data;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FrostAura.Libraries.Semantic.Core.Extensions.Configuration;

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
            .AddLogging()
            .AddHttpClient()
            .AddSemanticConfig(config)
            .AddSemanticServices()
            .AddAllSemanticThoughts();
	}

    /// <summary>
    /// Add all thoughts as services.
    /// </summary>
    /// <param name="services">Services collection to add to.</param>
    /// <returns>The ammended collection.</returns>
    private static IServiceCollection AddAllSemanticThoughts(this IServiceCollection services)
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
    /// Add all Semantic Kernel dependency configs.
    /// </summary>
    /// <param name="services">Services collection to add to.</param>
    /// <param name="config">Configuration to use.</param>
    /// <returns>The ammended collection.</returns>
    private static IServiceCollection AddSemanticConfig(this IServiceCollection services, SemanticConfig config)
    {
        return services
            .AddSingleton(Options.Create(config.ElevenLabsConfig))
            .AddSingleton(Options.Create(config.OpenAIConfig))
            .AddSingleton(Options.Create(config.PexelsConfig))
            .AddSingleton(Options.Create(config.GoogleConfig))
            .AddSingleton(Options.Create(config.PineconeConfig))
            .AddSingleton(Options.Create(config.FNBConfig))
            .AddSingleton(Options.Create(config.SemanticMemoryConfig))
            .AddSingleton(Options.Create(config));
    }

    /// <summary>
    /// Add all Semantic Kernel dependency services.
    /// </summary>
    /// <param name="services">Services collection to add to.</param>
    /// <returns>The ammended collection.</returns>
    private static IServiceCollection AddSemanticServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<ISemanticKernelLanguageModelsDataAccess, OpenAILanguageModelsDataAccess>();
    }
}
