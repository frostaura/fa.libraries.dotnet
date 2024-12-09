using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Polly;
using System.ComponentModel;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.IO;

/// <summary>
/// Web driver thoughts.
/// </summary>
public class WebDriverThoughts : BaseThought
{
    /// <summary>
    /// A function to call between when a page is loaded and read.
    /// </summary>
    public Func<WebDriver, Task> OnPageLoadedAsync;
    /// <summary>
    /// A function to call after the page read is done.
    /// </summary>
    public Func<WebDriver, Task> OnCleanupAsync;
    /// <summary>
    /// Configuration for the App config.
    /// </summary>
    private AppConfig _appConfig;

    /// <summary>
    /// Overloaded constructor to provide dependencies.
    /// </summary>
    /// <param name="serviceProvider">The dependency service provider.</param>
    /// <param name="semanticKernelLanguageModels">A component for communicating with language models.</param>
    /// <param name="appConfigOptions">Configuration for the App options.</param>
    /// <param name="logger">Instance logger.</param>
    public WebDriverThoughts(IServiceProvider serviceProvider, ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels, IOptions<AppConfig> appConfigOptions, ILogger<WebDriverThoughts> logger)
        : base(serviceProvider, semanticKernelLanguageModels, logger)
    {
        _appConfig = appConfigOptions
            .ThrowIfNull(nameof(appConfigOptions))
            .Value
            .ThrowIfNull(nameof(appConfigOptions));
    }

    /// <summary>
    /// Fetch a website's text content by loading it and waiting for all content (including lazy content), using a web driver.
    /// </summary>
    /// <param name="uri">The URI of the website's text to load.</param>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The lazy loaded website text.</returns>
    [KernelFunction, Description("Fetch a website's text content by loading it and waiting for all content (including lazy content), using a web driver.")]
    public virtual async Task<string> LoadWebsiteTextAsync(
        [Description("The URI of the website's text to load.")] string uri,
        CancellationToken token = default)
    {
        using (_logger.BeginScope("{MethodName}", nameof(LoadWebsiteTextAsync)))
        {
            uri.ThrowIfNullOrWhitespace(nameof(uri));
            _logger.LogInformation("Getting website text from URL '{URL}'.", uri);

            // Set Chrome options for headless mode
            var chromeOptions = new ChromeOptions();

            _logger.LogDebug("Using headless mode and a custom user agent string.");
            chromeOptions.AddArguments("--user-agent=Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 Edg/120.0.0.0");

            // Initialize the ChromeDriver with options
            using (var driver = new RemoteWebDriver(new Uri(_appConfig.SeleniumGridUrl), chromeOptions))
            {
                try
                {
                    // Navigate to the website.
                    _logger.LogDebug("Navigating to {URI}.", uri);
                    driver.Navigate().GoToUrl(uri);

                    // Wait for the page to load completely (you can adjust the timeout as needed).
                    _logger.LogDebug("Waiting for the document state to become complete.");
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

                    // Call the middleware, if any.
                    if (OnPageLoadedAsync is not null)
                    {
                        _logger.LogDebug("Calling OnPageLoadedAsync middleware, if any.");
                        await OnPageLoadedAsync.Invoke(driver);
                    }

                    // Extract all the text from the page.
                    var websiteText = await Policy
                        .Handle<Exception>()
                        .OrResult<string>(result => result.Length <= 0)
                        .WaitAndRetryAsync(5, retryCount => TimeSpan.FromSeconds(retryCount * 0.5))
                        .ExecuteAsync(() =>
                        {
                            var body = driver.FindElement(By.TagName("body"));

                            return Task.FromResult(body.Text);
                        });

                    _logger.LogInformation("Website loaded successfully ({ResponseCharCount} characters).", websiteText?.Length);

                    // Call the middleware, if any.
                    if (OnCleanupAsync is not null)
                    {
                        _logger.LogDebug("Calling OnCleanupAsync middleware, if any.");
                        await OnCleanupAsync.Invoke(driver);
                    }

                    return websiteText
                        .ThrowIfNullOrWhitespace(nameof(websiteText));
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    // Close the WebDriver.
                    _logger.LogInformation("Closing the web driver.");
                    driver.Quit();
                }
            };
        }
    }

    /// <summary>
    /// Search the internet for relevant and trusted sources of forecasts for a given asset and timeframe.
    /// </summary>
    /// <param name="assetName">The name or ticker of the asset. For example, BTC or Bitcoin.</param>
    /// <param name="timeframe">The timeframe for the forecast. For example, "for today", "for tomorrow" or "in 6 months".</param>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>A list of forecast URLs.</returns>
    [KernelFunction, Description("Search the internet for relevant and trusted sources of forecasts for a given asset and timeframe.")]
    public async Task<List<string>> SearchForecastsAsync(
        [Description("The name or ticker of the asset. For example, BTC or Bitcoin.")] string assetName,
        [Description("The timeframe for the forecast. For example, 'for today', 'for tomorrow' or 'in 6 months'.")] string timeframe,
        CancellationToken token = default)
    {
        using (_logger.BeginScope("{MethodName}", nameof(SearchForecastsAsync)))
        {
            var searchQuery = $"{assetName} forecast {timeframe}";
            var searchUrl = $"https://duckduckgo.com/?q={Uri.EscapeDataString(searchQuery)}";
            var forecastUrls = new List<string>();

            _logger.LogInformation("Searching for forecasts for {AssetName} {Timeframe} using DuckDuckGo.", assetName, timeframe);

            // Set Chrome options for headless mode
            var chromeOptions = new ChromeOptions();

            _logger.LogDebug("Using headless mode and a custom user agent string.");
            chromeOptions.AddArguments("--user-agent=Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 Edg/120.0.0.0");

            // Initialize the ChromeDriver with options
            using (var driver = new RemoteWebDriver(new Uri(_appConfig.SeleniumGridUrl), chromeOptions))
            {
                try
                {
                    // Navigate to the search URL.
                    _logger.LogDebug("Navigating to {SearchUrl}.", searchUrl);
                    driver.Navigate().GoToUrl(searchUrl);

                    // Wait for the page to load completely (you can adjust the timeout as needed).
                    _logger.LogDebug("Waiting for the document state to become complete.");
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

                    // Extract all the forecast URLs from the search results.
                    var searchResults = driver.FindElements(By.CssSelector(".result__a"));

                    foreach (var result in searchResults)
                    {
                        var url = result.GetAttribute("href");
                        forecastUrls.Add(url);
                    }

                    _logger.LogInformation("Found {ForecastCount} forecast URLs.", forecastUrls.Count);

                    return forecastUrls;
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    // Close the WebDriver.
                    _logger.LogInformation("Closing the web driver.");
                    driver.Quit();
                }
            };
        }
    }
}
