namespace FrostAura.Libraries.Core.Abstractions
{
    /// <summary>
    /// Allows for wrapping any type with Singleton implementation, providing a static instance of T.
    /// </summary>
    /// <typeparam name="T">The type of the object of the static instance.</typeparam>
    public class Singleton<T> where T : new()
    {
        /// <summary>
        /// Static instance of T.
        /// </summary>
        private static T _instance { get; set; }
        
        /// <summary>
        /// Object instance used for locking threads.
        /// </summary>
        private static object _lockObject { get; } = new object();

        /// <summary>
        /// Grab the static instance of T, in a thread-safe way. Create one if none exists.
        /// </summary>
        public T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                        }
                    }
                }
                
                return _instance;
            }
        }
    }
}