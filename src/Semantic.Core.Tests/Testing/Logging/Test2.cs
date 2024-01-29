using System.Runtime.CompilerServices;
using FrostAura.Libraries.Core.Extensions.Validation;
using Microsoft.Extensions.Logging;

namespace FrostAura.Libraries.Semantic.Core.Tests.Testing.Logging;

public class Test2
{
    private readonly Test3 _test3;
    private readonly ILogger _logger;

    public Test2(Test3 test3, ILogger<Test2> logger)
    {
        _logger = logger;
        _test3 = test3.ThrowIfNull(nameof(test3));
    }

    public async Task UpdateAsync()
    {
        using (BeginSemanticScope($"Scope Name Update {GetType().Name}"))
        {
            LogSemanticInformation($"Initializing update on {GetType().Name}...");
            await _test3.Update();
            LogSemanticInformation($"Again, update on {GetType().Name}...");
            await _test3.Update();
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
