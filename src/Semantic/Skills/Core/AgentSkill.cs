using Semantic.Consts.Cognitive;
using Semantic.Extensions.Cognitive;
using Semantic.Interfaces;

namespace Semantic.Skills.Core
{
    /// <summary>
    /// Take in any problem or query, $INPUT, and generate a comprehensive step by step plan for how to solve for $INPUT.
    /// </summary>
	public class TaskPlannerSkill : ISkill
	{
        /// <summary>
        /// The description of the expected input.
        /// </summary>
        public string InputDescription => "A goal to accomplish, a problem to solve or a high-level action to perform.";
        /// <summary>
        /// The function of the skill.
        /// </summary>
        public string Function => $"Take in any problem or query, {PromptVariables.INPUT}, and generate a comprehensive step by step plan for how to solve for {PromptVariables.INPUT}.";
        /// <summary>
        /// A skill that gives access to a LLM.
        /// </summary>
        private readonly ISkill _llmSkill;
        /// <summary>
        /// The foundational prompt to generate a plan for a query.
        /// </summary>
        private readonly string _prompt = $"""
            Create an XML plan step by step, to satisfy the goal given.
            To create a plan, follow these steps:
            0. The plan should be as short as possible.
            1. From a <goal> create a <plan> as a series of <functions>.
            2. Before using any function in a plan, check that it is present in the most recent [AVAILABLE FUNCTIONS] list. If it is not, do not use it. Do not assume that any function that was previously defined or used in another plan or in [EXAMPLES] is automatically available or compatible with the current plan.
            3. Only use functions that are required for the given goal.
            4. A function has an 'input' and an 'output'.
            5. The 'output' from each function is automatically passed as 'input' to the subsequent <function>.
            6. 'input' does not need to be specified if it consumes the 'output' of the previous function.
            7. To save an 'output' from a <function>, to pass into a future <function>, use <function.<<FunctionName>> ... setContextVariable: ""<UNIQUE_VARIABLE_KEY>""/>
            8. To save an 'output' from a <function>, to return as part of a plan result, use <function.<<FunctionName>> ... appendToResult: ""RESULT__<UNIQUE_RESULT_KEY>""/>
            9. Append an ""END"" XML comment at the end of the plan.

            [EXAMPLES]
            [AVAILABLE FUNCTIONS]
                <function.WriterSkill.Summarize function="Summarize a text blob." input="The text blob to summarize."/>
                <function.LanguageHelpers.TranslateTo function="Translate a text blob from English to 'translate_to_language'." input="The text blob to translate." translate_to_language="The language to translate the input text to."/>
                <function.EmailConnector.LookupContactEmail function="Look up a contact's email address." input="The identifier of the contact to look up."/>
                <function.EmailConnector.EmailTo function="Send an email to an email address." input="The text of the email body." recipient="The recipient email address."/>
            [END AVAILABLE FUNCTIONS]

            <goal>Summarize the input, then translate to japanese and email it to Martin</goal>
            <plan>
              <function.WriterSkill.Summarize/>
              <function.LanguageHelpers.TranslateTo translate_to_language="Japanese" setContextVariable="TRANSLATED_TEXT" />
              <function.EmailConnector.LookupContactEmail input="Martin" setContextVariable="CONTACT_RESULT" />
              <function.EmailConnector.EmailTo input="$TRANSLATED_TEXT" recipient="$CONTACT_RESULT"/>
            </plan>

            [END EXAMPLES]

            [AVAILABLE FUNCTIONS]
            {PromptVariables.AVAILABLE_FUNCTIONS}
            [END AVAILABLE FUNCTIONS]

            <goal>{PromptVariables.INPUT}</goal>
        """;

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="llmSkill">A skill that gives access to a LLM.</param>
        public TaskPlannerSkill(ISkill llmSkill)
		{
            _llmSkill = llmSkill;
		}

        /// <summary>
        /// Take in any problem or query, $INPUT, and generate a comprehensive step by step plan for how to solve for $INPUT.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="context">The chain execution context.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The output string.</returns>
        public async Task<string> RunAsync(string input, Dictionary<string, object> context, CancellationToken token = default)
        {
            context[PromptVariables.INPUT] = input;
            context[PromptVariables.AVAILABLE_FUNCTIONS] = input;

            var llmPrompt = _prompt.Contextualize(context);
            var llmResponse = await _llmSkill.RunAsync(llmPrompt, context, token);

            return llmResponse;
        }
    }
}
