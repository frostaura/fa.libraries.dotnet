using FrostAura.Libraries.Core.Extensions.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Semantic.Core.Attribute.Functions;
using Semantic.Core.Constants.Functions;
using Semantic.Core.Enumerations.Semantic;
using Semantic.Core.Extensions.Functions;
using Semantic.Core.Functions.Semantic;

namespace Semantic.Core.Functions.Procedural
{
    /// <summary>
    /// Take in a URL to perform an HTTP GET request on the input and return the stringified contents.
    /// </summary>
	public class HttpGet : FunctionCore
    {
        /// <summary>
        /// The purpose of the function or the kind of solution it provides.
        /// </summary>
        public override string Purpose => $"Take a URL to perform a HTTP GET request on and return the contents from the URL resource.";
        /// <summary>
        /// HTTP client factory.
        /// </summary>
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="httpClientFactory">HTTP client factory.</param>
        /// <param name="logger">Instance logger.</param>
        public HttpGet(IHttpClientFactory httpClientFactory, ILogger logger)
            : base(logger)
        {
            _httpClientFactory = httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
        }

        /// <summary>
        /// The purpose of the function or the kind of solution it provides.
        /// </summary>
        /// <param name="arguments">The required arguments provided.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The output string.</returns>
        [Argument(ArgumentNames.INPUT, $"The absolute URL of the HTTP resource to get.")]
        public override async Task<string> ExecuteAsync(Dictionary<string, string> arguments, CancellationToken token = default)
        {
            var input = arguments.GetArgument(ArgumentNames.INPUT);

            using (var client = _httpClientFactory.CreateClient())
            {
                var response = await client.GetAsync(input, token);
                var responseStr = await response.Content.ReadAsStringAsync(token);

                return responseStr;
            }
        }
    }
}
