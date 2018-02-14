using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using FrostAura.Libraries.Security.OAuth.Models.Google;
using Newtonsoft.Json;

namespace FrostAura.Libraries.Security.OAuth.Providers
{
    /// <summary>
    /// Google OAuth provider implementation.
    /// </summary>
    public sealed class GoogleOAuthProvider : BaseOAuthProvider
    {
        /// <summary>
        /// Unique provider identifier.
        /// </summary>
        public override string Identifier { get; } = "Google";
        
        /// <summary>
        /// HTTP service to use.
        /// </summary>
        private IHttpService _httpService { get; }

        /// <summary>
        /// Constructor to allow passing of parameters.
        /// </summary>
        /// <param name="httpService">Application HTTP client.</param>
        /// <param name="clientId">Unique platform client identifier.</param>
        /// <param name="clientSecret">Gets the client secret.</param>
        /// <param name="scope">Gets the scope.</param>
        /// <param name="redirectUrl">Redirect URL from concent screen.</param>
        public GoogleOAuthProvider(IHttpService httpService,
            string clientId,
            string clientSecret,
            string scope =
                "https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email",
            string redirectUrl = null)
            : base(clientId, clientSecret, scope, redirectUrl)
        {
            _httpService = httpService.ThrowIfNull(nameof(httpService));
        }
        
        /// <summary>
        /// Construct consent URL.
        /// </summary>
        /// <returns>Consent GET URL.</returns>
        public override string GetConsentUrl()
        {
            return "https://accounts.google.com/o/oauth2/v2/auth?" +
                  $"scope={Uri.EscapeDataString(_scope)}&" +
                  $"redirect_uri={_redirectUrl}&" +
                  "response_type=code&" +
                  $"client_id={_clientId}&" +
                  $"consumerKey={_clientId}&" +
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
            var request = new HttpRequestMessage(HttpMethod.Post, "https://www.googleapis.com/oauth2/v3/token")
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
                "https://www.googleapis.com/plus/v1/people/me?access_token=" + authToken.ThrowIfNullOrWhitespace(nameof(authToken)));
            HttpRequest httpRequest = request
                .ToHttpRequest();
            HttpResponse<GoogleProfileModel> httpResponse = await _httpService
                .RequestAsync<GoogleProfileModel>(httpRequest, token);

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
                    .Name?
                    .GivenName,
                Email = httpResponse
                    .Response?
                    .Emails?
                    .First()
                    .Value,
                Lastname = httpResponse
                    .Response?
                    .Name?
                    .FamilyName,
                ProfileImageUrl = httpResponse
                    .Response?
                    .Image?
                    .Url
                    .Replace("50", "256"),
                ProviderSpecificProfile = httpResponse
                    .Response
            };
        }
    }
}