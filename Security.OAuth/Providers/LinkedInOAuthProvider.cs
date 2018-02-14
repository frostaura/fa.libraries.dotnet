using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Http.Extensions;
using FrostAura.Libraries.Http.Interfaces;
using FrostAura.Libraries.Http.Models.Requests;
using FrostAura.Libraries.Http.Models.Responses;
using FrostAura.Libraries.Security.OAuth.Abstractions;
using FrostAura.Libraries.Security.OAuth.Enums;
using FrostAura.Libraries.Security.OAuth.Models;
using FrostAura.Libraries.Security.OAuth.Models.Facebook;
using FrostAura.Libraries.Security.OAuth.Models.Google;
using FrostAura.Libraries.Security.OAuth.Models.LinkedIn;
using Newtonsoft.Json;

namespace FrostAura.Libraries.Security.OAuth.Providers
{
    /// <summary>
    /// Google OAuth provider implementation.
    /// </summary>
    public sealed class LinkedInOAuthProvider : BaseOAuthProvider
    {
        /// <summary>
        /// Unique provider identifier.
        /// </summary>
        public override string Identifier { get; } = "LinkedIn";
        
        /// <summary>
        /// HTTP service to use.
        /// </summary>
        private IHttpService _httpService { get; }
        
        /// <summary>
        /// Unique state for each provider instance.
        /// </summary>
        private string _state { get; }

        /// <summary>
        /// Constructor to allow passing of parameters.
        /// </summary>
        /// <param name="httpService">Application HTTP client.</param>
        /// <param name="clientId">Unique platform client identifier.</param>
        /// <param name="clientSecret">Gets the client secret.</param>
        /// <param name="scope">Gets the scope.</param>
        /// <param name="state">Unique immutable state per application.</param>
        /// <param name="redirectUrl">Redirect URL from concent screen.</param>
        public LinkedInOAuthProvider(IHttpService httpService,
            string clientId,
            string clientSecret,
            string scope = "r_basicprofile r_emailaddress",
            string state = "FrostAura09051991",
            string redirectUrl = null)
            : base(clientId, clientSecret, scope, redirectUrl)
        {
            _httpService = httpService.ThrowIfNull(nameof(httpService));
            _state = state.ThrowIfNullOrWhitespace(nameof(state));
        }
        
        /// <summary>
        /// Construct consent URL.
        /// </summary>
        /// <returns>Consent GET URL.</returns>
        public override string GetConsentUrl()
        {
            return "https://www.linkedin.com/uas/oauth2/authorization?" +
                  $"scope={Uri.EscapeDataString(_scope)}&" +
                  $"redirect_uri={_redirectUrl}&" +
                  "response_type=code&" +
                  $"client_id={_clientId}&" +
                  $"consumerKey={_clientId}&" +
                  $"state={_state}&" +
                  "display=popup";
        }

        /// <summary>
        /// Transform the code received from the concent acceptance into a usable token.
        /// </summary>
        /// <param name="code">Concent acceptance code.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Auth token.</returns>
        protected override async Task<string> GetAuthTokenFromConcentCodeAsync(string code, CancellationToken token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://www.linkedin.com/oauth/v2/accessToken")
            {
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", code.ThrowIfNullOrWhitespace(nameof(code))),
                    new KeyValuePair<string, string>("redirect_uri", _redirectUrl),
                    new KeyValuePair<string, string>("client_id", _clientId),
                    new KeyValuePair<string, string>("client_secret", _clientSecret)
                })
            };
            HttpRequest httpRequest = request
                .ToHttpRequest();
            HttpResponse<AuthTokenResponse> httpResponse = await _httpService
                .RequestAsync<AuthTokenResponse>(httpRequest, token);

            if (!httpResponse.IsOk)
            {
                Status.Value = new StatusModel
                {
                    Status = OperationStatus.Error,
                    StatusText = "Failed to get auth token from code.",
                    Detail = httpResponse.ResponseMessage
                };
                
                return null;
            }
            
            return httpResponse
                .Response?
                .Access_token;
        }

        /// <summary>
        /// Get the user profile from an auth token.
        /// </summary>
        /// <param name="authToken">Auth token retrieved from concent code exchange.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>User profile information.</returns>
        protected override async Task<UserProfileModel> GetProfileAsync(string authToken, CancellationToken token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                "https://api.linkedin.com/v1/people/~:(id,first-name,last-name,summary,picture-url,email-address)?format=json&oauth2_access_token=" + authToken.ThrowIfNullOrWhitespace(nameof(authToken)));
            HttpRequest httpRequest = request
                .ToHttpRequest();
            HttpResponse<LinkedInProfileModel> httpResponse = await _httpService
                .RequestAsync<LinkedInProfileModel>(httpRequest, token);
            
            if (!httpResponse.IsOk)
            {
                Status.Value = new StatusModel
                {
                    Status = OperationStatus.Error,
                    StatusText = "Failed to get profile.",
                    Detail = httpResponse.ResponseMessage
                };
                
                return null;
            }
            
            return new UserProfileModel
            {
                FirstName = httpResponse
                    .Response?
                    .FirstName,
                Email = httpResponse
                    .Response?
                    .EmailAddress,
                Lastname = httpResponse
                    .Response?
                    .LastName,
                ProfileImageUrl = httpResponse
                    .Response?
                    .PictureUrl,
                ProviderSpecificProfile = httpResponse
                    .Response
            };
        }
    }
}