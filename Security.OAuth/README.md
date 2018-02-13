# fa.standard.libraries.security.oauth
FrostAura set of OAuth 2.0 abstractions for ease of use across multiple platforms.
## Providers
| Provider | Status |
| --- | --- |
| Facebook | PENDING |
| Google | SUPPORTED |
| LinkedIn | PENDING |

## Requirements
On each respective provider platform, you are required to set up an application with a valid redirect URL.
The redirect URL will be an endpoing in your WebAPI or MVC project that the provider redirects to after successful or failed authentication.
When setting up an application with the provider, you will get an app id and secret. Keep a reference to these for later.

## How To
### Setup
- Install-Package FrostAura.Libraries.Security.OAuth
- Create an instance of 'IHttpService'. The providers require this for construction.
```
IHttpService HttpService = new JsonHttpService();
```
- Create an instance of 'BaseOAuthProvider'.
```
BaseOAuthProvider provider = new GoogleOAuthProvider(HttpService, 
  "MY_CLIENT_ID",
  "MY_CLIENT_SECRET",
  returnUrl: "MY_RETURN_URL");
  
provider.Status.Subscribe((StatusModel newState) =>
{
  // Handle provider status updates here.
  // 'OperationStatus.ProfileInformationFetched' is the final event with profile information.
});
```
### Generating Consent Screen URL
At this point we want to generate a concent screen URL to redirect to in order for our user to sign in and redirect back to us after.
```
string consentUrl = provider.GetConsentUrl();
```
At this point you want to redirect the user to this URL.
- For Xamarin, you would want to have a WebView and set the source to this URL.
- For WebAPI & MVC Core, simply redirect your client to this URL as you will get back to your app via the return URL you set up with the provider in the requirements step.

### Processing Return URL
When auth was successful or failed, a redirect will occur to your specified return url with a query string parameter appended for the auth code or the error.
- For Xamarin, we simply want to tap into the OnNavigating event of the WebView and process the URL.
```
public async void HandleWebViewNavigating(object sender, WebNavigatingEventArgs e)
{
    await provider.ProcessUrlAsync(e.Url, CancellationToken.None);
}
```
- For WebAPI & MVC Core, you will need an endpoint that's route matches what you specified as the return URL.
```
[HttpGet]
public async IActionResult Get([FromQuery] string code, [FromQuery] string code, CancellationToken token)
{
    // You could at this point handle the code and error strings manually or simply let our provider do it.
    var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
    string url = location.AbsoluteUri;
    
    await provider.ProcessUrlAsync(url, token);
}
```

## Contribute
In order to contribute, simply fork the repository, make changes and create a pull request.

## Support
For any queries, contact deanmar@outlook.com.
