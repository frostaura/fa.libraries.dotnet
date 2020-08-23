using FrostAura.Libraries.Communication.Models.SMS;

namespace FrostAura.Libraries.Communication.Interfaces
{
    /// <summary>
    /// Contract for a SMS communication service.
    /// </summary>
    public interface ISmsCommunicator : ICommunicator<SmsRequest, SmsResponse>
    {
    }
}
