using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace FrostAura.Libraries.Semantic.Core.Tests.Testing.Logging;

public class Test3
{
    private readonly ILogger _logger;

    public Test3(ILogger<Test3> logger)
    {
        _logger = logger;
    }

    public async Task Update()
    {
        using (BeginSemanticScope($"Scope Name Update {GetType().Name}"))
        {
            LogSemanticInformation("Ping");
            await Task.Delay(TimeSpan.FromSeconds(1));
            LogSemanticInformation("Pong");
            await Task.Delay(TimeSpan.FromSeconds(1));
            LogSemanticInformation("Pang");
            await Task.Delay(TimeSpan.FromSeconds(1));
            LogSemanticInformation("Pung");
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
