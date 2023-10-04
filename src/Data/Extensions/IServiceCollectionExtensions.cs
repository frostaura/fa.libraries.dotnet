using FrostAura.Libraries.Data.Interfaces;
using FrostAura.Libraries.Data.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FrostAura.Libraries.Data.Extensions
{
    /// <summary>
    /// Service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add FrostAura components data access services to the DI container.
        /// </summary>
        /// <param name="services">Application services collection.</param>
        /// <returns>Application services collection.</returns>
        public static IServiceCollection AddFrostAuraComponentsData(this IServiceCollection services)
        {
            return services
                .AddSingleton<IContentService, EmbeddedContentService>();
        }
    }
}
