using Microsoft.SemanticKernel.ChatCompletion;

namespace FrostAura.Libraries.Semantic.Core.Models.Prompts;

/// <summary>
/// A response to a prompt of an LLM with contextual information. 
/// </summary>
public class Conversation
{
    /// <summary>
    /// The text content of the last response.
    /// </summary>
    public string LastMessage { get; set; }
    /// <summary>
    /// The conversation chat history.
    /// </summary>
    public ChatHistory ChatHistory { get; set; } = new ChatHistory();
    /// <summary>
    /// A delegate to allow for calling back into a model.
    /// </summary>
    public Func<string, Task<string>> CallModel { get;set;}

    /// <summary>
    /// Continue the conversation with a follow-up question / query.
    /// </summary>
    /// <param name="prompt">The LLM prompt.</param>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The response string.</returns>
    public async Task<string> ChatAsync(string prompt, CancellationToken token)
    {
        var response = await CallModel(prompt);
        LastMessage = response;

        return response;
    }

    /// <summary>
    /// Override what happens on when the ToString method gets called.
    /// </summary>
    /// <returns>The string representation of the response.</returns>
    public override string ToString()
    {
        return LastMessage;
    }
}
