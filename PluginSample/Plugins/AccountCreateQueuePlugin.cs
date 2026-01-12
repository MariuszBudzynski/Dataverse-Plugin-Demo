using PluginSample.Services;

namespace PluginSample.Plugins
{
    public class AccountCreateQueuePlugin : BasePlugin
    {
        protected override void ExecuteInternal()
        {
            if (Context is null)
                return;

            var queue = new FakeMessageQueue(Tracing);
            var publisher = new QueuePublisher(queue);

            publisher.Publish(Context);
        }
    }
}