using System.Reflection;
using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;
using FrostAura.Libraries.Semantic.Core.Data;
using FrostAura.Libraries.Semantic.Core.Data.Logging;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
    /// Get a though instance from it's name.
    /// </summary>
    /// <param name="serviceProvider">The service provider for dependency injection.</param>
    /// <param name="name">Name of the action.</param>
    /// <returns>The action instance.</returns>
    public static TThoughtType GetThoughtByName<TThoughtType>(this IServiceProvider serviceProvider, string name)
        where TThoughtType: BaseThought
    {
        return (TThoughtType)serviceProvider
            .GetThoughtByName(name);
    }

    /// <summary>
    /// Add Semantic Core services by first creating a default configuration context.
    /// </summary>
    /// <param name="services">Services collection to add to.</param>
    /// <param name="configuration">Application configuration to use.</param>
    /// <returns>The ammended collection.</returns>
    public static IServiceCollection AddSemanticCore(this IServiceCollection services, out IConfigurationRoot configuration)
    {
        configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        return services.AddSemanticCore(configuration);
    }

    /// <summary>
    /// Add Semantic Core services.
    /// </summary>
    /// <param name="services">Services collection to add to.</param>
    /// <param name="configuration">Application configuration to use.</param>
    /// <param name="includeCoreSkills">Whether to include the core FrostAura functions.</param>
    /// <returns>The ammended collection.</returns>
    public static IServiceCollection AddSemanticCore(this IServiceCollection services, IConfigurationRoot configuration, bool includeCoreSkills = true)
	{
        var semanticConfig = new SemanticConfig();

        configuration
            .GetSection(nameof(SemanticConfig))
            .Bind(semanticConfig);

        var response = services
            .AddLogging()
            .AddSingleton<ILoggerProvider, HierarchicalLoggerProvider>()
            .AddHttpClient()
            .AddSemanticConfig(semanticConfig.ThrowIfNull(nameof(semanticConfig)))
            .AddSemanticServices(semanticConfig);

        if (includeCoreSkills) response.AddSemanticThoughts(Assembly.GetAssembly(typeof(BaseThought)));

        return response;
	}

    /// <summary>
    /// Add all thoughts as services from a particular assembly. These would be classes that inherits BaseThought.
    /// </summary>
    /// <param name="services">Services collection to add to.</param>
    /// <returns>The ammended collection.</returns>
    public static IServiceCollection AddSemanticThoughts(this IServiceCollection services, Assembly assembly)
    {
        assembly
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
            .AddSingleton(Options.Create(config.MediumConfig))
            .AddSingleton(Options.Create(config.AppConfig))
            .AddSingleton(Options.Create(config.ImgbbConfig))
            .AddSingleton(Options.Create(config));
    }

    /// <summary>
    /// Add all Semantic Kernel dependency services.
    /// </summary>
    /// <param name="services">Services collection to add to.</param>
    /// <param name="config">Configuration to use.</param>
    /// <returns>The ammended collection.</returns>
    private static IServiceCollection AddSemanticServices(this IServiceCollection services, SemanticConfig config)
    {
        return services
            .AddSingleton<ISemanticKernelLanguageModelsDataAccess, OpenAILanguageModelsDataAccess>();
    }
}
