using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FrostAura.Libraries.Core.Extensions.Reactive;
using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Core.Interfaces.Reactive;
using FrostAura.Libraries.Security.OAuth.Enums;
using FrostAura.Libraries.Security.OAuth.Models;

namespace FrostAura.Libraries.Security.OAuth.Abstractions
{
    /// <summary>
    /// Base for an OAuth provider.
    /// </summary>
    public abstract class BaseOAuthProvider
    {
        /// <summary>
        /// Unique provider identifier.
        /// </summary>
        public abstract string Identifier { get; }
        
        /// <summary>
        /// Observable status of the provider.
        /// </summary>
        public IObservedValue<StatusModel> Status { get; }
        
        /// <summary>
        /// Unique platform client identifier.
        /// </summary>
        protected string _clientId { get; }

        /// <summary>
        /// Gets the client secret.
        /// </summary>
        protected string _clientSecret { get; }

        /// <summary>
        /// Redirect URL from concent screen.
        /// </summary>
        protected string _redirectUrl { get; } = "https://redirector.frostaura.net/application/oauth-return";
        
        /// <summary>
        /// Gets the scope.
        /// </summary>
        protected string _scope { get; }

        /// <summary>
        /// Constructor to allow passing of parameters.
        /// </summary>
        /// <param name="clientId">Unique platform client identifier.</param>
        /// <param name="clientSecret">Gets the client secret.</param>
        /// <param name="scope">Gets the scope.</param>
        /// <param name="redirectUrl">Redirect URL from concent screen.</param>
        protected BaseOAuthProvider(string clientId,
            string clientSecret,
            string scope,
            string redirectUrl)
        {
            _clientId = clientId.ThrowIfNullOrWhitespace(nameof(clientId));
            _clientSecret = clientSecret.ThrowIfNullOrWhitespace(nameof(clientSecret));
            _scope = scope.ThrowIfNullOrWhitespace(nameof(scope));

            if (!string.IsNullOrWhiteSpace(redirectUrl)) _redirectUrl = redirectUrl;

            Status = new StatusModel
            {
                Status = OperationStatus.Idle,
                StatusText = $"{Identifier} Initialized"
            }.AsObservedValue();
        }

        /// <summary>
        /// Action for processing a return URL.
        /// This extracts the code or error if any and call the success or error event subscriptions respectively.
        /// </summary>
        /// <param name="url">URL to attempt processing on.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Whether or not the URL was processed.</returns>
        public async Task<bool> ProcessUrlAsync(string url, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            
            var code = ExtractFromUrl(url, "code");

            if (!string.IsNullOrWhiteSpace(code))
            {
                Status.Value = new StatusModel
                {
                    Status = OperationStatus.AuthTokenRetrieving,
                    StatusText = "Retrieving Auth Token"
                };

                string authToken = await GetAuthTokenFromConcentCodeAsync(code, token);
                
                Status.Value = new StatusModel
                {
                    Status = OperationStatus.AuthTokenRetrieved,
                    StatusText = "Auth Token Retrieved",
                    Detail = authToken
                };
                Status.Value = new StatusModel
                {
                    Status = OperationStatus.ProfileInformationFetching,
                    StatusText = "Fetching Profile",
                    Detail = code
                };

                UserProfileModel profile = await GetProfileAsync(authToken, token);
                
                Status.Value = new StatusModel
                {
                    Status = OperationStatus.ProfileInformationFetched,
                    StatusText = "Profile Fetched",
                    Detail = profile
                };

                return true;
            };

            var error = ExtractFromUrl(url, "error");

            if (!string.IsNullOrWhiteSpace(error))
            {
                Status.Value = new StatusModel
                {
                    Status = OperationStatus.Error,
                    StatusText = "Sign In Failed",
                    Detail = error
                };
                
                return true;
            };
            
            return false;
        }

        /// <summary>
        /// Construct consent URL.
        /// </summary>
        /// <returns>Consent GET URL.</returns>
        public abstract string GetConsentUrl();

        /// <summary>
        /// Transform the code received from the concent acceptance into a usable token.
        /// </summary>
        /// <param name="code">Concent acceptance code.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Auth token.</returns>
        protected abstract Task<string> GetAuthTokenFromConcentCodeAsync(string code, CancellationToken token);

        /// <summary>
        /// Get the user profile from an auth token.
        /// </summary>
        /// <param name="authToken">Auth token retrieved from concent code exchange.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>User profile information.</returns>
        protected abstract Task<UserProfileModel> GetProfileAsync(string authToken, CancellationToken token);
        
        /// <summary>
        /// Extracts a code or error from URL. If none, return null.
        /// </summary>
        /// <returns>Code or error from URL. If none, return null.</returns>
        /// <param name="url">URL.</param>
        /// <param name="parameterName">Query string param to look for.</param>
        private string ExtractFromUrl(string url, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            var uri = new Uri(url);
            var queryStringParameter = uri
                .Query
                .Split('&')
                .FirstOrDefault(p => p.Contains(parameterName + "="));

            return queryStringParameter?.Split('=')[1];
        }
    }
}