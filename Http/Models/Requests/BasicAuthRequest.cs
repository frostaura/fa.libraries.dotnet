namespace FrostAura.Libraries.Http.Models.Requests
{
    /// <summary>
    /// Basic authorization model.
    /// </summary>
    public class BasicAuthRequest
    {
        /// <summary>
        /// Username used in the request.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password used in the request.
        /// </summary>
        public string Password { get; set; }
    }
}