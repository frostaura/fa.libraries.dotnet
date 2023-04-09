using System;
using Finance.Models;

namespace Finance.Interfaces.Managers
{
	/// <summary>
	/// A manager for projection features. This could be for wealth or other numbers over time.
	/// </summary>
	public interface IProjectionManager
	{
		/// <summary>
		/// Project to a specific date.
		/// </summary>
		/// <param name="request">Required projection request data.</param>
		/// <param name="targetDate">The target date to project to.</param>
		/// <returns>The projection data.</returns>
		Task<ProjectionResponse> ProjectToDateAsync(ProjectionRequest request, DateTime targetDate);
        /// <summary>
        /// Project to a specific net worth. For example until a balance of 100 000 has been achieved.
        /// </summary>
        /// <param name="request">Required projection request data.</param>
        /// <param name="targetNetWorth">The target net worth to project to.</param>
        /// <returns>The projection data.</returns>
        Task<ProjectionResponse> ProjectToNetWorthAsync(ProjectionRequest request, double targetNetWorth);
        /// <summary>
        /// Project till a provided delegate determines the projection is terminal.
        /// </summary>
        /// <param name="request">Required projection request data.</param>
        /// <param name="isTerminalDelegate">The determiner for when to stop the projection.</param>
        /// <returns>The projection data.</returns>
        Task<ProjectionResponse> ProjectTillIsTerminalAsync(ProjectionRequest request, Func<int, DateTime, bool> isTerminalDelegate);
    }
}

