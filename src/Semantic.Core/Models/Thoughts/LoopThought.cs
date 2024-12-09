using System.Collections.Generic;

namespace FrostAura.Libraries.Semantic.Core.Models.Thoughts
{
    /// <summary>
    /// A thought model that supports looping over collections.
    /// </summary>
    public class LoopThought : Thought
    {
        /// <summary>
        /// The key of the collection to iterate over.
        /// </summary>
        public string CollectionKey { get; set; }

        /// <summary>
        /// The key to assign each item in the collection to.
        /// </summary>
        public string ItemKey { get; set; }

        /// <summary>
        /// The nested thoughts to execute for each item in the collection.
        /// </summary>
        public List<Thought> NestedThoughts { get; set; }
    }
}
