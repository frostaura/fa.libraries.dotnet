using System.Net.Http;
using System.Xml;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Semantic.Consts.Cognitive;
using Semantic.Extensions.Cognitive;
using Semantic.Interfaces;

namespace Semantic.Skills.IO
{
    /// <summary>
    /// Take in a URL to perform an HTTP GET request on the input and return the stringified contents.
    /// </summary>
	public class HttpGetSkill : BaseSkill
	{
        /// <summary>
        /// The description of the expected input.
        /// </summary>
        public override string InputDescription => "The absolute URL of the HTTP resource to get.";
        /// <summary>
        /// The function of the skill.
        /// </summary>
        public override string Function => $"Take a URL, {PromptVariables.INPUT}, to perform a HTTP GET request on and return the contents from the URL resource.";

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="availableSkills">All available skills.</param>
        public HttpGetSkill(IEnumerable<BaseSkill> availableSkills, ILogger<HttpGetSkill> logger)
            :base(availableSkills, logger)
		{ }

        /// <summary>
        /// Take in any problem or query, $INPUT, and generate a comprehensive step by step plan for how to solve for $INPUT.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="context">The chain execution context.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The output string.</returns>
        protected override async Task<string> ExecuteAsync(string input, Dictionary<string, string> context, CancellationToken token = default)
        {
            context[PromptVariables.INPUT] = input;

            using(var client = new HttpClient())
            {
                var response = await client.GetAsync(input);
                var responseStr = await response.Content.ReadAsStringAsync();

                return responseStr;
            }
        }
    }
}
