using System.Collections.Concurrent;

namespace FrostAura.Libraries.Semantic.Core.Data.Logging;

public class ScopeDisposer : IDisposable
{
    private readonly int _operationId;
    private readonly ConcurrentDictionary<int, List<string>> _scopeInformation;

    public ScopeDisposer(int operationId, ConcurrentDictionary<int, List<string>> scopeInformation)
    {
        _operationId = operationId;
        _scopeInformation = scopeInformation;
    }

    public void Dispose()
    {
        // Clean up the scope information when the scope is disposed
        _scopeInformation.TryRemove(_operationId, out _);
    }
}
