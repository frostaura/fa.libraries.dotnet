namespace FrostAura.Libraries.Security.OAuth.Models.Google
{
    /// <summary>
    /// Entity that represents the user's name data we receive from the Google API.
    /// </summary>
    public class GoogleName
    {
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
    }
}