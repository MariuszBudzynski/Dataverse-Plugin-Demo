using Microsoft.Xrm.Sdk;
using PluginSample.Services.DTO;
using PluginSample.Services.Interfaces;

namespace PluginSample.Services
{
    public class QueuePublisher
    {
        private readonly IMessageQueue _queue;

        public QueuePublisher(IMessageQueue queue)
        {
            _queue = queue;
        }

        public void Publish(IPluginExecutionContext context)
        {
            var message = new QueueMessage
            {
                EntityName = context.PrimaryEntityName,
                EntityId = context.PrimaryEntityId,
                MessageName = context.MessageName,
                Stage = context.Stage,
                UserId = context.UserId
            };

            _queue.Send(message);
        }
    }
}