using FrostAura.Libraries.Security.OAuth.Enums;

namespace FrostAura.Libraries.Security.OAuth.Models
{
    /// <summary>
    /// Operation status model.
    /// </summary>
    public class StatusModel
    {
        /// <summary>
        /// Current status of the operation.
        /// </summary>
        /// <value>The status.</value>
        public OperationStatus Status { get; set; }

        /// <summary>
        /// Status text update.
        /// </summary>
        /// <value>The status text.</value>
        public string StatusText { get; set; }

        /// <summary>
        /// Additional detail about the operation like error and success data.
        /// </summary>
        public object Detail { get; set; }
    }
}