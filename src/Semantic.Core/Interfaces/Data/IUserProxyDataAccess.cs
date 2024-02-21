using FrostAura.Libraries.Semantic.Core.Models.Logging;

namespace FrostAura.Libraries.Semantic.Core.Interfaces.Data;

/// <summary>
/// A proxy service requirements for a user proxy data.
/// </summary>
public interface IUserProxyDataAccess
{
	/// <summary>
	/// Ask the user proxy a question and get an instance of the completion source for when the question has been answered.
	/// </summary>
	/// <param name="question">Question to ask the user proxy.</param>
	/// <param name="token">Token to cancel downstream operations.</param>
	/// <returns>The answer from the user.</returns>
	Task<string> AskUserAsync(string question, CancellationToken token);
	/// <summary>
	/// A handler for when a system event occurs.
	/// </summary>
	/// <param name="scopes">A collection of all the scopes in the event system.</param>
	/// <param name="currentEvent">The current occurance / most recent event.</param>
	/// <returns>Void</returns>
	Task OnEventAsync(List<LogItem> scopes, LogItem currentEvent);
}