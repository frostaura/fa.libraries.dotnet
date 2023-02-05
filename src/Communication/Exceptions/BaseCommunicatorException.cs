using Newtonsoft.Json;
using System;

namespace FrostAura.Libraries.Communication.Exceptions
{
    /// <summary>
    /// Base exception for communicator errors.
    /// </summary>
    public abstract class BaseCommunicatorException : Exception
    {
        public BaseCommunicatorException(object context)
            :base(JsonConvert.SerializeObject(context))
        { }
    }
}
