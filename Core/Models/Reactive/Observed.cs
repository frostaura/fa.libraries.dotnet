using System;
using FrostAura.Libraries.Core.Interfaces.Reactive;

namespace FrostAura.Libraries.Core.Models.Reactive
{
    /// <summary>
    /// A container for a single value with an event for when it's setter is called.
    /// </summary>
    public class Observed<T> : IObservedValue<T>
    {
        /// <summary>
        /// Private instance of the value.
        /// </summary>
        private T _value { get; set; }
        
        /// <summary>
        /// Event that should get fired upon the changing of the value.
        /// </summary>
        private event Action<T> _onValueChanged;

        /// <summary>
        /// The instance of the value.
        /// </summary>
        public T Value
        {
            get => _value;
            set
            {
                if(object.Equals(_value, value)) return;
                
                _value = value;
                
                _onValueChanged?.Invoke(value);
            }
        }

        /// <summary>
        /// Event that should get fired upon the changing of the value.
        /// </summary>
        public void Subscribe(Action<T> onChangedHandler)
        {
            _onValueChanged += onChangedHandler;

            if (!object.Equals(_value, default(T)))
            {
                onChangedHandler(_value);
            } 
        }
    }
}