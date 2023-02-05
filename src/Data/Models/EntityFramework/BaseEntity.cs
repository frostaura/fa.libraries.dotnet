using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace FrostAura.Libraries.Data.Models.EntityFramework
{
    /// <summary>
    /// A base for all entities with auto-generated unique identifiers.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Unique identifier of entity.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Whether or not the entity is flagged as deleted.
        /// </summary>
        [Required]
        [JsonIgnore]
        public bool Deleted { get; set; } = false;

        /// <summary>
        /// The time at which the entity was created.
        /// </summary>
        [Required]
        [JsonIgnore]
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}