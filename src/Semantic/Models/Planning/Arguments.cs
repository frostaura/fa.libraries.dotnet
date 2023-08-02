using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Semantic.Models.Planning
{
    /// <summary>
    /// An argument that should be set as a context variable to a step.
    /// </summary>
    [DebuggerDisplay("{Name} - {SuggestedApproach}")]
    public class Argument
    {
        /// <summary>
        /// The suggested approach for implementing the skill.
        /// </summary>
        [XmlAttribute("argument")]
        public string SuggestedApproach { get; set; }
    }
}
