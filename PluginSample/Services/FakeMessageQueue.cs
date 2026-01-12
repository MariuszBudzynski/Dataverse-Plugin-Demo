using Microsoft.Xrm.Sdk;
using PluginSample.Services.DTO;
using PluginSample.Services.Interfaces;

namespace PluginSample.Services
{
    public class FakeMessageQueue : IMessageQueue
    {
        private readonly ITracingService _tracing;

        public FakeMessageQueue(ITracingService tracing)
        {
            _tracing = tracing;
        }

        public void Send(QueueMessage message)
        {
            _tracing.Trace(
                $"[FAKE QUEUE] {message.MessageName} | {message.EntityName} | {message.EntityId}"
            );
        }
    }
}
