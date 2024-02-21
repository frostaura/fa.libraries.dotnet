using FrostAura.Libraries.Core.Extensions.Validation;

namespace FrostAura.Libraries.Semantic.Core.Data.Adapters;

/// <summary>
/// An adapter for IDisposable actions.
/// </summary>
public class DisposableAdapter : IDisposable
{
    /// <summary>
    /// The action to invoke upon disposing.
    /// </summary>
    private readonly Action _onDisposing;

    /// <summary>
    /// An overloaded constructor to allow for passing arguments.
    /// </summary>
    /// <param name="onDisposing">The action to invoke upon disposing.</param>
	public DisposableAdapter(Action onDisposing)
	{
        _onDisposing = onDisposing
            .ThrowIfNull(nameof(onDisposing));

    }

    /// <summary>
    /// The function to invoke the disposing action.
    /// </summary>
    public void Dispose()
    {
        _onDisposing.Invoke();
    }
}
