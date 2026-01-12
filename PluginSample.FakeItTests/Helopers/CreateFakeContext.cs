using FakeXrmEasy;
using Microsoft.Xrm.Sdk;

namespace PluginSample.FakeItTests.Helopers
{
    public static class CreateFakeContext
    {
        public static XrmFakedPluginExecutionContext CreateFakePluginContext(Entity entity, string messageType, XrmFakedContext fakeContext)
        {
            var pluginContext = fakeContext.GetDefaultPluginContext();
            pluginContext.MessageName = messageType;
            pluginContext.InputParameters = new ParameterCollection
            {
                {"Target", entity }
            };

            return pluginContext;
        }
    }
}
