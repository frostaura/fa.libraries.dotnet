using System.Diagnostics;
using Newtonsoft.Json;

namespace FrostAura.Libraries.Semantic.Core.Models.Thoughts
{
    /// <summary>
    /// A thought model.
    /// </summary>
    [DebuggerDisplay("{Reasoning} => ${OutputKey}: {Action}")]
	public class Thought
	{
        /// <summary>
        /// The unique key to assign the output of the thought's action to.
        /// </summary>
        public string OutputKey { get; set; }
        /// <summary>
        /// The reasoning for the throught to exist.
        /// </summary>
        public string Reasoning { get; set; }
        /// <summary>
        /// The next action to take.
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// The arguments for the action.
        /// </summary>
        public Dictionary<string, string> Arguments { get; set; }
        /// <summary>
        /// The critisism on the thought.
        /// </summary>
        public string Critisism { get; set; } = "None";
        /// <summary>
        /// The response from the action taken.
        /// </summary>
        public string Observation { get; set; } = "Pending";

        /// <summary>
        /// Overridden string display for the thought.
        /// </summary>
        /// <returns>The formatted thought string.</returns>
        public override string ToString()
        {
            var argumentsStr = JsonConvert.SerializeObject(Arguments, Formatting.Indented);

            return $"""


                Thought: {Reasoning}
                Critisism: {Critisism}
                Action: {Action}
                Arguments:
                {argumentsStr}
                Observation: {Observation}

                """;
        }
    }
}

