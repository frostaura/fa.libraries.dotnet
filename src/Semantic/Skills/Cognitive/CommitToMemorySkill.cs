using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Semantic.Consts.Cognitive;
using Semantic.Interfaces;
using Semantic.Models.LLMs;
using Semantic.Models.LLMs.OpenAI;

namespace Semantic.Skills.Cognitive
{
    /// <summary>
    /// Allows for passing {PromptVariables.INPUT}, a text blob that should be stored in a knowledge base (typically a vector database like Pinecone) which can provide future queries with context as well as being queried using the RecallFromMemorySkill.
    /// </summary>
    public class CommitToMemorySkill : BaseSkill
    {
        /// <summary>
        /// The description of the expected input.
        /// </summary>
        public override string InputDescription => "A natural language query, question or statement to call the LLM with.";
        /// <summary>
        /// The function of the skill.
        /// </summary>
        public override string Function => $"Allows for passing {PromptVariables.INPUT}, a text blob that should be stored in a knowledge base (typically a vector database like Pinecone) which can provide future queries with context as well as being queried using the RecallFromMemorySkill.";

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="availableSkills">All available skills.</param>
        public LLMSkill(IEnumerable<BaseSkill> availableSkills, ILogger logger)
            : base(availableSkills, logger)
        { }

        /// <summary>
        /// Allows for passing {PromptVariables.INPUT} to an OpenAI large language model and returning the model's reponse.
        /// </summary>
        /// <param name="input">The input string to pass to the large language model.</param>
        /// <param name="context">The chain execution context.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The output string.</returns>
        protected override async Task<string> ExecuteAsync(string input, Dictionary<string, string> context, CancellationToken token = default)
        {
            using(var client = new HttpClient())
            {
                var modelName = "gpt-4-32k";
                var apiVersion = "2023-03-15-preview";
                var domain = "der-aic-devtest-openai-labs.openai.azure.com";
                var apiKey = "2ee7a39ce2c54595911ba5a3b1b10c14";
                var url = $"https://{domain}/openai/deployments/{modelName}/chat/completions?api-version={apiVersion}";
                var request = new
                {
                    messages = new dynamic[]
                    {
                        new
                        {
                            role = "user",
                            content = input
                        }
                    },
                    max_tokens = 8000,
                    temperature = 0.5,
                    frequency_penalty = 0,
                    presence_penalty = 0,
                    top_p = 0.95
                };
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                requestMessage.Headers.Add("api-key", apiKey);
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                var responseMessage = await client.SendAsync(requestMessage, token);
                var responseStr = await responseMessage.Content.ReadAsStringAsync(token);

                try
                {
                    var response = JsonConvert.DeserializeObject<OpenAILLMResponse>(responseStr);

                    return response.Choices.First().Message.Content;
                }
                catch (Exception ex)
                {
                    throw new ArgumentOutOfRangeException("An error occured. If unsure, this is most likely due to the input model size being reached.", ex);
                }
            }
        }
    }
}