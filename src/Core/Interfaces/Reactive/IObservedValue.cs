using System;

namespace FrostAura.Libraries.Core.Interfaces.Reactive
{
    /// <summary>
    /// A container for a single value with an event for when it's setter is called.
    /// </summary>
    public interface IObservedValue<T>
    {
        /// <summary>
        /// Event that should get fired upon the changing of the value.
        /// </summary>
        void Subscribe(Action<T> onChangedHandler);
        
        /// <summary>
        /// The instance of the value.
        /// </summary>
        T Value { get; set; }
    }
}