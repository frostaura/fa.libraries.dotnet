using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FrostAura.Libraries.Core.Models.Graphics
{
    /// <summary>
    /// Red, Green & Blue container model.
    /// </summary>
    [DebuggerDisplay("Red: {R}, Green: {G}, Blue: {B}")]
    public class RGB
    {
        /// <summary>
        /// Red.
        /// </summary>
        [Range(minimum: 0, maximum: 255)]
        public int R { get; set; }
        
        /// <summary>
        /// Green.
        /// </summary>
        [Range(minimum: 0, maximum: 255)]
        public int G { get; set; }
        
        /// <summary>
        /// Blue.
        /// </summary>
        [Range(minimum: 0, maximum: 255)]
        public int B { get; set; }
    }
}