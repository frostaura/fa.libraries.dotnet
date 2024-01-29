using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
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
    /// Overloaded constructor to provide dependencies.
    /// </summary>
    /// <param name="serviceProvider">The dependency service provider.</param>
    /// <param name="semanticKernelLanguageModels">A component for communicating with language models.</param>
    /// <param name="logger">Instance logger.</param>
    public WebDriverThoughts(IServiceProvider serviceProvider, ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels, ILogger<WebDriverThoughts> logger)
        : base(serviceProvider, semanticKernelLanguageModels, logger)
    { }

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
        using (BeginSemanticScope(nameof(LoadWebsiteTextAsync)))
        {
            uri.ThrowIfNullOrWhitespace(nameof(uri));
            LogSemanticInformation($"Getting website text from URL '{uri}'.");

            // Set Chrome options for headless mode
            var chromeOptions = new ChromeOptions();

            LogSemanticDebug("Using headless mode and a custom user agent string.");
            chromeOptions.AddArguments("--headless");
            chromeOptions.AddArguments("--user-agent=Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 Edg/120.0.0.0");

            // Initialize the ChromeDriver with options
            using (var driver = new ChromeDriver(chromeOptions))
            {
                // Navigate to the website.
                LogSemanticDebug($"Navigating to '{uri}'.");
                driver.Navigate().GoToUrl(uri);

                // Wait for the page to load completely (you can adjust the timeout as needed).
                LogSemanticDebug($"Waiting for the document state to become complete.");
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

                // Call the middleware, if any.
                if (OnPageLoadedAsync is not null)
                {
                    LogSemanticDebug($"Calling OnPageLoadedAsync middleware, if any.");
                    await OnPageLoadedAsync.Invoke(driver);
                }

                // Extract all the text from the page.
                var websiteText = driver.FindElement(By.TagName("body")).Text;

                LogSemanticDebug($"Website loaded successfully ({websiteText.Length} characters).");

                // Call the middleware, if any.
                if (OnCleanupAsync is not null)
                {
                    LogSemanticDebug($"Calling OnCleanupAsync middleware, if any.");
                    await OnCleanupAsync.Invoke(driver);
                }

                // Close the WebDriver.
                LogSemanticDebug($"Closing the web driver.");
                driver.Quit();

                return websiteText
                    .ThrowIfNullOrWhitespace(nameof(websiteText));
            };
        }
    }
}