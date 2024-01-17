namespace FrostAura.Libraries.Semantic.Core.Interfaces.Data
{
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
	}
}
