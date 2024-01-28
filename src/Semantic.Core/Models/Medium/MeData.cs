namespace FrostAura.Libraries.Semantic.Core.Models.Medium;

/// <summary>
/// Response data for me object.
/// </summary>
public class MeData
{
	/// <summary>
	/// Medium user id.
	/// </summary>
	public string Id { get; set; }
    /// <summary>
    /// Medium username.
    /// </summary>
    public string Username { get; set; }
    /// <summary>
    /// Medium user dsiplay name.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Medium profile URL.
    /// </summary>
    public string Url { get; set; }
    /// <summary>
	/// Medium profile image URL.
	/// </summary>
	public string ImageUrl { get; set; }
}
