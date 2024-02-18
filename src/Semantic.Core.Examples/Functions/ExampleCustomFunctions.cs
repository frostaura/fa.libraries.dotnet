using System.ComponentModel;
using FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Semantic.Core.Examples.Functions;

/// <summary>
/// Example custom functions.
/// </summary>
public class ExampleCustomFunctions : BaseThought
{
    /// <summary>
    /// Overloaded constructor to provide dependencies.
    /// </summary>
    /// <param name="serviceProvider">The dependency service provider.</param>
    /// <param name="semanticKernelLanguageModels">A component for communicating with language models.</param>
    /// <param name="logger">Instance logger.</param>
    public ExampleCustomFunctions(IServiceProvider serviceProvider, ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels, ILogger<ExampleCustomFunctions> logger)
        : base(serviceProvider, semanticKernelLanguageModels, logger)
    { }

    /// <summary>
    /// Get the current system time.
    /// </summary>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The response body as a string.</returns>
    [KernelFunction, Description("Get the current system time.")]
    public async Task<string> GetTimeAsync(
        //[Description("Example Argument")] string exampleArgument,
        CancellationToken token = default)
    {
        return DateTime.Now.ToLongTimeString();
    }

    /// <summary>
    /// Get the current system time.
    /// </summary>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The response body as a string.</returns>
    [KernelFunction, Description("Get the system time X hour and Y days from now. Both arguments are required.")]
    public async Task<string> GetFutureTimeAsync(
        [Description("int:mandatory:The count of hours to get the time for in the future. This argument MUST be provided and NON_ZERO (Ask the user if required).")] string hours,
        [Description("int:mandatory:The count of days to get the time for in the future. This argument MUST be provided and NON_ZERO (Ask the user if required).")] string days, 
        CancellationToken token = default)
    {
        var hoursInFuture = int.Parse(hours);
        var daysInFuture = int.Parse(days);

        return DateTime.Now.AddDays(daysInFuture).AddHours(hoursInFuture).ToLongTimeString();
    }
}
