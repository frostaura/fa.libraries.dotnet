using System.Diagnostics;

namespace Finance.Models
{
	/// <summary>
	/// All information returned for a projection.
	/// </summary>
	[DebuggerDisplay("[{ProjectionEndDate}] Net Worth: {NetWorth}")]
	public class ProjectionResponse
	{
		/// <summary>
		/// This marks the date the projection ended. This could be due to a goal being achieved.
		/// </summary>
		public DateTime ProjectionEndDate { get; set; }
		/// <summary>
		/// Net worth amount.
		/// </summary>
		public double NetWorth { get; set; }
		/// <summary>
		/// The augmented accounts of the original request.
		/// </summary>
        public ProjectionRequest AugmentedRequest { get; set; }

    }
}

