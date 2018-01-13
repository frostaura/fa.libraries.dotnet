using System.ComponentModel;

namespace MediaServer.Plex.Enums
{
    /// <summary>
    /// Plex HTTP endpoint enums with a placeholder for the FQDN.
    /// E.g. http://192.168.0.5:32400
    /// NOTE: An additional '/' at the end of the server address should be removed as it can cause 404's.
    /// </summary>
    public enum Endpoint
    {
        [Description("https://plex.tv/users/sign_in.json")]
        SignIn,
        
        [Description("{0}/:/prefs")]
        ServerPreferences,
        
        [Description("{0}/library/sections")]
        Libraries,
        
        [Description("{0}/library/sections/{1}/all")]
        LibraryMovies
    }
}