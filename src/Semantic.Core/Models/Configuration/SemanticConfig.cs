namespace Semantic.Core.Models.Configuration
{
    /// <summary>
    /// Configuration for Pinecone vector db.
    /// </summary>
    public class PineconeConfig
    {
		/// <summary>
		/// The environment / hosting location.
		/// </summary>
		public string Environment { get; set; }
		/// <summary>
		/// The API key for security.
		/// </summary>
		public string ApiKey { get; set; }
        /// <summary>
		/// Index name.
		/// </summary>
		public string IndexName { get; set; }
        /// <summary>
		/// Index namespace.
		/// </summary>
		public string Namespace { get; set; }
    }
}

