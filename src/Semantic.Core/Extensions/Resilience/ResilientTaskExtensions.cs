using Polly;

namespace FrostAura.Libraries.Semantic.Core.Extensions.Resilience
{
	public static class ResilientTaskExtensions
	{
        /// <summary>
        /// A generic retry policy for all Tasks.
        /// </summary>
        /// <param name="originalTask"></param>
        /// <returns>A wrapped task with added resilience.</returns>
		public static Task AsResilientTask(this Task originalTask)
        {
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(5, retryCount => TimeSpan.FromSeconds(retryCount * 1.5))
                .ExecuteAsync(async () => await originalTask);
        }

        /// <summary>
        /// A generic retry policy for all Tasks.
        /// </summary>
        /// <typeparam name="T">Type of the task.</typeparam>
        /// <param name="originalTask"></param>
        /// <returns>A wrapped task with added resilience.</returns>
		public static Task<T> AsResilientTask<T>(this Task<T> originalTask)
		{
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(5, retryCount => TimeSpan.FromSeconds(retryCount * 1.5))
                .ExecuteAsync(async () => await originalTask);
        }
	}
}
