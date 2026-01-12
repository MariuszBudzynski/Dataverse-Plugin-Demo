using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PluginSample.Plugins
{
    public class AccountUpdatePrimaryContactPlugin : BasePlugin
    {
        protected override void ExecuteInternal()
        {
            if (Context.MessageName != "Update")
                return;

            if (!(Context.InputParameters.TryGetValue("Target", out var target)
                  && target is Entity entity))
                return;

            if (entity.LogicalName != "account")
                return;

            // Retrive full account becouse target has only changed fields)
            var account = Service.Retrieve("account", entity.Id, new ColumnSet("primarycontactid"));

            if (!account.Contains("primarycontactid"))
                return;

            // Retrive link to contact
            var contactRef = account.GetAttributeValue<EntityReference>("primarycontactid");

            // Update contact
            var contact = new Entity("contact", contactRef.Id);
            contact["description"] = "Updated because parent account was updated";

            Service.Update(contact);

            Tracing.Trace($"Updated primary contact {contactRef.Id} for account {entity.Id}");
        }
    }
}