using FrostAura.Libraries.Core.Extensions.Validation;

namespace FrostAura.Libraries.Semantic.Core.Models.Prompts;

/// <summary>
/// The CO-STAR framework guides you to provide all of the crucial pieces of information about your task to the LLM in a structured manner, ensuring a tailored and optimized response to exactly what you need.
///
/// Examples Context: Write a facebook post to advertise my company’s new product. My company’s name is Alpha and the product is called Beta, a new ultra-fast hairdryer.
/// </summary>
public class CoStarPrompt
{
    /// <summary>
    /// Provide background information on the task.
    ///
    /// Example: I want to advertise my company’s new product. My company’s name is Alpha and the product is called Beta, which is a new ultra-fast hairdryer.
    /// </summary>
    private readonly string _context;
    /// <summary>
    /// Define what the task is that you want the LLM to perform.
    ///
    /// Example: Create a Facebook post for me, which aims to get people to click on the product link to purchase it.
    /// </summary>
    private readonly string _objective;
    /// <summary>
    /// Specify the writing style you want the LLM to use.
    ///
    /// Example: Follow the writing style of successful companies that advertise similar products, such as Dyson.
    /// </summary>
    private readonly string _style;
    /// <summary>
    /// Set the attitude of the response.
    ///
    /// Example: Persuasive
    /// </summary>
    private readonly string _tone;
    /// <summary>
    /// Identify who the response is intended for.
    ///
    /// Example: My company’s audience profile on Facebook is typically the older generation. Tailor your post to target what this audience typically looks out for in hair products.
    /// </summary>
    private readonly string _audience;
    /// <summary>
    /// Provide the response format.
    ///
    /// Example: The Facebook post, kept concise yet impactful.
    /// </summary>
    private readonly string _response;

    /// <summary>
    /// Overloaded constructor to provide dependencies.
    /// </summary>
    /// <param name="context">Provide background information on the task.</param>
    /// <param name="objective">Define what the task is that you want the LLM to perform.</param>
    /// <param name="style">Specify the writing style you want the LLM to use.</param>
    /// <param name="tone">Set the attitude of the response.</param>
    /// <param name="audience">Identify who the response is intended for.</param>
    /// <param name="response">Provide the response format.</param>
    public CoStarPrompt(
        string context,
        string objective,
        string style,
        string tone,
        string audience,
        string response
        )
    {
        _context = context.ThrowIfNullOrWhitespace(nameof(context));
        _objective = objective.ThrowIfNullOrWhitespace(nameof(objective));
        _style = style.ThrowIfNullOrWhitespace(nameof(style));
        _tone = tone.ThrowIfNullOrWhitespace(nameof(tone));
        _audience = audience.ThrowIfNullOrWhitespace(nameof(audience));
        _response = response.ThrowIfNullOrWhitespace(nameof(response));
    }

    /// <summary>
    /// Serialize the prompt to a string.
    /// </summary>
    /// <returns>The prompt.</returns>
    public override string ToString()
    {
        return $"""
        # CONTEXT #
        {_context}

        # OBJECTIVE #
        {_objective}

        # STYLE #
        {_style}

        # TONE #
        {_tone}

        # AUDIENCE #
        {_audience}

        # RESPONSE #
        {_response}
        """.Trim();
    }
}
