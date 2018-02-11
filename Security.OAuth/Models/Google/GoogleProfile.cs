using System.Collections.Generic;

namespace FrostAura.Libraries.Security.OAuth.Models.Google
{
    /// <summary>
    /// Entity that represents the data we can retrieve from the Google peoples API.
    /// </summary>
    public class GoogleProfile
    {
        public string Gender { get; set; }
        public List<GoogleEmail> Emails { get; set; } = new List<GoogleEmail>();
        public GoogleName Name { get; set; }
        public GoogleImage Image { get; set; }
        public List<GoogleOrganization> Organizations { get; set; } = new List<GoogleOrganization>();
    }
}