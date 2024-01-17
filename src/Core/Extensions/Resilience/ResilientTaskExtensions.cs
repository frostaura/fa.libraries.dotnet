using Polly;

namespace FrostAura.Libraries.Core.Extensions.Resilience
{
	public static class ResilientTaskExtensions
	{
        /// <summary>
        /// A generic retry policy for all Tasks.
        /// </summary>
        /// <param name="originalTask">The original work that should be executed in the retry loop.</param>
        /// <param name="maxRetryCount">The count of retries to attempt before raising the internal exception.</param>
        /// <param name="retryExponent">The exponent for exponential backoff.</param>
        /// <returns>A wrapped task with added resilience.</returns>
		public static Task AsResilientTask(this Task originalTask, int maxRetryCount = 5, double retryExponent = 1.5)
        {
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(maxRetryCount, retryCount => TimeSpan.FromSeconds(retryCount * retryExponent))
                .ExecuteAsync(async () => await originalTask);
        }

        /// <summary>
        /// A generic retry policy for all Tasks.
        /// </summary>
        /// <typeparam name="T">Type of the task.</typeparam>
        /// <param name="originalTask">The original work that should be executed in the retry loop.</param>
        /// <param name="maxRetryCount">The count of retries to attempt before raising the internal exception.</param>
        /// <param name="retryExponent">The exponent for exponential backoff.</param>
        /// <returns>A wrapped task with added resilience.</returns>
		public static Task<T> AsResilientTask<T>(this Task<T> originalTask, int maxRetryCount = 5, double retryExponent = 1.5)
		{
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(maxRetryCount, retryCount => TimeSpan.FromSeconds(retryCount * retryExponent))
                .ExecuteAsync(async () => await originalTask);
        }
	}
}
