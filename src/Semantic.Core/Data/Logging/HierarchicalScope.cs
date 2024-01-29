using System.Collections.Concurrent;

namespace FrostAura.Libraries.Semantic.Core.Data.Logging;

/// <summary>
/// A scope object that the hierarchical logger uses to keep state.
/// </summary>
public class HierarchicalScope : IDisposable
{
    /// <summary>
    /// 
    /// </summary>
    private readonly int _operationId;
    /// <summary>
    /// The actual state of the scope instance.
    /// </summary>
    private readonly ConcurrentDictionary<int, List<string>> _scopeInformation;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="operationId"></param>
    /// <param name="scopeInformation">The initial state of the scope instance.</param>
    public HierarchicalScope(int operationId, ConcurrentDictionary<int, List<string>> scopeInformation)
    {
        _operationId = operationId;
        _scopeInformation = scopeInformation;
    }

    /// <summary>
    /// Clean up unmanaged resources manually.
    /// </summary>
    public void Dispose()
    {
        // Clean up the scope information when the scope is disposed.
        _scopeInformation.TryRemove(_operationId, out _);
    }
}
