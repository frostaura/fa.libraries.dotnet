using System.Reflection;
using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Data.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FrostAura.Libraries.Data.Services
{
    /// <summary>
    /// A data access service to manipulate and fetch embedded content.
    /// </summary>
    public class EmbeddedContentService : IContentService
    {
        /// <summary>
        /// Instance logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Overloaded constructr to allow for dependency injection.
        /// </summary>
        /// <param name="logger">Instance logger.</param>
        public EmbeddedContentService(ILogger<EmbeddedContentService> logger)
        {
            _logger = logger
                .ThrowIfNull(nameof(logger));
        }

        /// <summary>
        /// Get content by key and parse it to a desired type.
        /// </summary>
        /// <typeparam name="TParsedContentResult">Type which to cast the content to if found.</typeparam>
        /// <param name="key">Key whihc to look up the content for.</param>
        /// <param name="assembly">Assembly to load the content from.</param>
        /// <param name="token">Cancellation token to cancel downstream operations if required.</param>
        /// <returns>Parsed content or default.</returns>
        public async Task<TParsedContentResult> GetContentByKeyAsync<TParsedContentResult>(string key, Assembly assembly, CancellationToken token)
        {
            try
            {
                var resourcePath = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(key));

                using (var stream = assembly.GetManifestResourceStream(resourcePath))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var resultString = await reader.ReadToEndAsync();

                        _logger.LogDebug($"Getting embedded content with key '{key}' succeeded.");

                        if (typeof(TParsedContentResult) == typeof(string)) return (TParsedContentResult)(object)resultString;

                        var result = JsonConvert.DeserializeObject<TParsedContentResult>(resultString);

                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Getting embedded content with key '{key}' failed: {e.Message}", e);
            }

            return default;
        }
    }
}
