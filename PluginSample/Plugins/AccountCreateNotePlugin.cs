using Microsoft.Xrm.Sdk;

namespace PluginSample.Plugins
{
    public class AccountCreateNotePlugin : BasePlugin
    {
        // Note intentionally not linked to the account – this is a simplified example.

        protected override void ExecuteInternal()
        {
            if (Context.MessageName != "Create")
                return;

            if (!(Context.InputParameters.TryGetValue("Target", out var target) && target is Entity entity))
                return;

            if (entity.LogicalName != "account")
                return;

            var note = new Entity("annotation");
            note["subject"] = "Account created";
            note["notetext"] = "This note was created automatically by a plugin.";
            Service.Create(note);

            Tracing.Trace(
                $"AccountCreateNotePlugin executed. Updated entity {entity.LogicalName} with id {entity.Id} "
                + $"in stage {Context.Stage} by user {Context.InitiatingUserId}");
        }
    }
}