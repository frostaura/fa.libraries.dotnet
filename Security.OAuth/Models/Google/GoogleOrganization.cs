namespace FrostAura.Libraries.Security.OAuth.Models.Google
{
    /// <summary>
    /// Entity that represents the user's organizational data we receive from the Google API.
    /// </summary>
    public class GoogleOrganization
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Primary { get; set; }
    }
}