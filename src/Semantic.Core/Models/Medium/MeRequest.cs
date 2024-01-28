namespace FrostAura.Libraries.Semantic.Core.Models.Medium;

public class MeData
{
	public string Id { get; set; }
	public string Username { get; set; }
	public string Name { get; set; }
	public string Url { get; set; }
	public string ImageUrl { get; set; }
}

public class MeResponse
{
	/// <summary>
	/// The response data.
	/// </summary>
	public MeData Data { get; set; }
}
