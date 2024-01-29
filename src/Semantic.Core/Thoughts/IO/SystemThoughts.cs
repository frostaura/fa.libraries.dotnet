using System.ComponentModel;
using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.IO;

/// <summary>
/// System thoughts.
/// </summary>
public class SystemThoughts : BaseThought
{
    /// <summary>
    /// A proxy service requirements for a user proxy service.
    /// </summary>
    private readonly IUserProxyDataAccess _userProxy;

    /// <summary>
    /// Overloaded constructor to provide dependencies.
    /// </summary>
    /// <param name="serviceProvider">The dependency service provider.</param>
    /// <param name="semanticKernelLanguageModels">A component for communicating with language models.</param>
    /// <param name="userProxy">A proxy service requirements for a user proxy service.</param>
    /// <param name="logger">Instance logger.</param>
    public SystemThoughts(IServiceProvider serviceProvider, ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels, IUserProxyDataAccess userProxy, ILogger<SystemThoughts> logger)
        : base(serviceProvider, semanticKernelLanguageModels, logger)
    {
        _userProxy = userProxy.ThrowIfNull(nameof(userProxy));
    }

    /// <summary>
    /// Ask the end-user a question, question, feedback etc and return the end-user response.
    /// </summary>
    /// <param name="question">The question to ask the end-user.</param>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The response from the end-user.</returns>
    [KernelFunction, Description("Ask the end-user a question, question, feedback etc and return the end-user response.")]
    public Task<string> AskForInputAsync(
        [Description("The question to ask the end-user.")] string question,
        CancellationToken token = default)
    {
        using (BeginSemanticScope(nameof(AskForInputAsync)))
        {
            LogSemanticInformation($"Asking the user a question: '{question}'");

            return _userProxy
                .AskUserAsync(question.ThrowIfNullOrWhitespace(nameof(question)), token);
        }
    }

    /// <summary>
    /// Output text content to the end-user.
    /// </summary>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The response body as a string.</returns>
    [KernelFunction, Description("Output text content to the end-user.")]
    public Task<string> OutputTextAsync(
        [Description("The text to output.")] string output,
        CancellationToken token = default)
    {
        using (BeginSemanticScope(nameof(OutputTextAsync)))
        {
            LogSemanticInformation($"Returning output to the user: '{output}'");

            return Task.FromResult(output.ThrowIfNullOrWhitespace(nameof(output)));
        }
    }
}