namespace FrostAura.Libraries.Core.Settings
{
    /// <summary>
    /// ForstAura core resillience settings.
    /// </summary>
    public class Resillience
    {
        /// <summary>
        /// How long to wait in between retries.
        /// </summary>
        public const int RETRY_TIMEOUT = 30;
        
        /// <summary>
        /// How many times a request is allowed to be retried.
        /// </summary>
        public const int RETRY_COUNT = 5;
    }
}