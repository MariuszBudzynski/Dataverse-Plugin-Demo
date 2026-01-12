using Microsoft.Xrm.Sdk;

namespace PluginSample.Plugins
{
    public class AccountUpdatePlugin : BasePlugin
    {
        protected override void ExecuteInternal()
        {
            if (Context.MessageName != "Update")
                return;

            if (!(Context.InputParameters.TryGetValue("Target", out var target) && target is Entity entity))
                return;

            if (entity.LogicalName != "account")
                return;

            entity["description"] = "Updated by plugin";

            Tracing.Trace($"Update entity {entity.LogicalName} with id {entity.Id}");
        }
    }
}
