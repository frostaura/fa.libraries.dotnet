using FrostAura.Libraries.Core.Extensions.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.SkillDefinition;
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
        public Action<WebDriver> OnPageLoaded;

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
        [SKFunction, Description("Fetch a website's text content by loading it and waiting for all content (including lazy content), using a web driver.")]
        public async Task<string> LoadTextAsync(
            [Description("The URI of the website's text to load.")] string uri,
            CancellationToken token = default)
        {
            uri.ThrowIfNullOrWhitespace(nameof(uri));

            // Set Chrome options for headless mode
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--headless"); // Run headless

            // Initialize the ChromeDriver with options
            using (var driver = new ChromeDriver(chromeOptions))
            {
                // Navigate to the TechCrunch website
                driver.Navigate().GoToUrl(uri);

                // Wait for the page to load completely (you can adjust the timeout as needed)
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

                // Call the middleware, if any.
                OnPageLoaded?.Invoke(driver);

                // Extract all the text from the page
                var allText = driver.FindElement(By.TagName("body")).Text;

                // Close the WebDriver
                driver.Quit();

                return allText;
            };
        }
    }
}