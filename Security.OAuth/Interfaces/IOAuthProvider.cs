using FrostAura.Libraries.Core.Interfaces.Reactive;

namespace FrostAura.Libraries.Security.OAuth.Interfaces
{
    /// <summary>
    /// OAuth provider interface.
    /// </summary>
    public interface IOAuthProvider
    {
        /// <summary>
        /// Display title or provider.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Root URL for concent screen.
        /// </summary>
        string Url { get; }

        /// <summary>
        /// Unique platform client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        string ClientId { get; }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        string Scope { get; }

        /// <summary>
        /// Gets the client secret.
        /// </summary>
        string ClientSecret { get; }

        /// <summary>
        /// Action for processing a return URL.
        /// This extracts the code or error if any and call the success or error event subscriptions respectively.
        /// </summary>
        void ProcessUrl(string url);
    }
}