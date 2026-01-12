using PluginSample.Services.DTO;

namespace PluginSample.Services.Interfaces
{
    public interface IMessageQueue
    {
        void Send(QueueMessage message);
    }
}