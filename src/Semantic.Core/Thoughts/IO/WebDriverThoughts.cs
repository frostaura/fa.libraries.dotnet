using FrostAura.Libraries.Core.Extensions.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.ComponentModel;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.IO
{
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
        /// <param name="logger">Instance logger.</param>
        public WebDriverThoughts(ILogger<WebDriverThoughts> logger)
            : base(logger)
        { }

        /// <summary>
        /// Fetch a website's text content by loading it and waiting for all content (including lazy content), using a web driver.
        /// </summary>
        /// <param name="uri">The URI of the website's text to load.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>The lazy loaded website text.</returns>
        [KernelFunction, Description("Fetch a website's text content by loading it and waiting for all content (including lazy content), using a web driver.")]
        public virtual async Task<string> LoadTextAsync(
            [Description("The URI of the website's text to load.")] string uri,
            CancellationToken token = default)
        {
            uri.ThrowIfNullOrWhitespace(nameof(uri));

            // Set Chrome options for headless mode
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--headless");

            // Initialize the ChromeDriver with options
            using (var driver = new ChromeDriver(chromeOptions))
            {
                // Navigate to the TechCrunch website
                driver.Navigate().GoToUrl(uri);

                // Wait for the page to load completely (you can adjust the timeout as needed)
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

                // Call the middleware, if any.
                if (OnPageLoadedAsync is not null) await OnPageLoadedAsync.Invoke(driver);

                // Extract all the text from the page
                var allText = driver.FindElement(By.TagName("body")).Text;

                // Call the middleware, if any.
                if (OnCleanupAsync is not null) await OnCleanupAsync.Invoke(driver);

                // Close the WebDriver
                driver.Quit();

                return allText;
            };
        }
    }
}