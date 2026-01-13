using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PluginSample.Plugins
{
    public class AccountDeletePlugin : BasePlugin
    {
        protected override void ExecuteInternal()
        {
            if (Context.MessageName != "Delete")
                return;

            // Ensure plugin runs in PreOperation
            if (Context.Stage != 20) 
                return;

            // Validate Target
            if (!(Context.InputParameters.TryGetValue("Target", out var targetObj) && targetObj is EntityReference target))
                return;

            if (target.LogicalName != "account")
                return;

            Tracing.Trace($"Deleting related records for account: {target.Id}");

            // QueryExpression to find related child records
            var query = new QueryExpression("new_childentity")
            {
                ColumnSet = new ColumnSet(false), // we only need Id
                NoLock = true
            };

            //Filter
            query.Criteria.AddCondition("new_parentaccountid", ConditionOperator.Equal, target.Id);

            var results = Service.RetrieveMultiple(query);

            Tracing.Trace($"Found {results.Entities.Count} related child records to delete.");

            foreach (var child in results.Entities)
            {
                Service.Delete(child.LogicalName, child.Id); Tracing.Trace($"Deleted child record: {child.Id}");
            }
        }
    }
}