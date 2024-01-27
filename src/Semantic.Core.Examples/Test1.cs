using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Semantic.Core.Examples;

public class Test1
{
    private readonly Test2 _test2;
    private readonly ILogger _logger;

    public Test1(Test2 test2, ILogger<Test1> logger)
	{
        _logger = logger;
        LogSemanticInformation($"Creating class {GetType().Name}.");
        _test2 = test2;
    }

    public async Task UpdateAsync()
    {
        using (BeginSemanticScope($"Scope Name Update {GetType().Name}"))
        {
            LogSemanticInformation($"Initializing update on {GetType().Name}...");
            await _test2.UpdateAsync();
            LogSemanticInformation("Update done!");
        }
    }

    private IDisposable BeginSemanticScope(string groupDescription)
    {
        return _logger.BeginScope(new KeyValuePair<string, object>(groupDescription, this));
    }

    private void LogSemanticInformation(string message)
    {
        var operationId = RuntimeHelpers.GetHashCode(this);

        _logger.LogInformation($"[[[{operationId}]]]{message}");
    }
}
