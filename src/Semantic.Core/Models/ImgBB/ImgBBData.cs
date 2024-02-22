using System.Diagnostics;

namespace FrostAura.Libraries.Semantic.Core.Models.ImgBB;

/// <summary>
/// ImgBB API data.
/// </summary>
[DebuggerDisplay("{Url}")]
public class ImgBBData
{
    /// <summary>
    /// The URL of the image.
    /// </summary>
    public string Url { get; set; }
}
