namespace FrostAura.Libraries.Security.OAuth.Models.Google
{
    /// <summary>
    /// Entity that represents the user's name data we receive from the Google API.
    /// </summary>
    public class GoogleNameModel
    {
        /// <summary>
        /// Last name.
        /// </summary>
        public string FamilyName { get; set; }
        
        /// <summary>
        /// First name.
        /// </summary>
        public string GivenName { get; set; }
    }
}