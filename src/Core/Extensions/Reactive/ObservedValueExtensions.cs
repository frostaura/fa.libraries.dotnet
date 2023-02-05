using System.Runtime.CompilerServices;
using FrostAura.Libraries.Core.Interfaces.Reactive;
using FrostAura.Libraries.Core.Models.Reactive;

namespace FrostAura.Libraries.Core.Extensions.Reactive
{
    /// <summary>
    /// Extensions for the observed values.
    /// </summary>
    public static class ObservedValueExtensions
    {
        public static IObservedValue<T> AsObservedValue<T>(this T obj)
        {
            var instance = new Observed<T>
            {
                Value = obj
            };

            return instance;
        }
    }
}