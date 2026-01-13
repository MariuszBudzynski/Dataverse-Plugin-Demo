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

        public static XrmFakedPluginExecutionContext CreateFakePluginContext(EntityReference entityReference, string messageType, XrmFakedContext fakeContext, int stage)
        {
            var pluginContext = fakeContext.GetDefaultPluginContext();
            pluginContext.MessageName = messageType;
            pluginContext.Stage = stage;
            pluginContext.InputParameters = new ParameterCollection
            {
                {"Target", entityReference }
            };

            return pluginContext;
        }
    }
}
