using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FrostAura.Libraries.Data.Models.EntityFramework
{
    /// <summary>
    /// A base for all entities with auto-generated unique identifiers and names.
    /// </summary>
    [DebuggerDisplay("Name: {Name}")]
    public class BaseNamedEntity : BaseEntity
    {
        /// <summary>
        /// Entity name / short description.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = $"A valid name is required.")]
        public string Name { get; set; } = string.Empty;
    }
}