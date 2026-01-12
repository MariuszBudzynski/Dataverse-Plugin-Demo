using Microsoft.Xrm.Sdk;
using System;

namespace PluginSample
{
    public abstract class BasePlugin : IPlugin
    {
        protected IOrganizationService Service { get; private set; }
        protected IPluginExecutionContext Context { get; private set; }
        protected ITracingService Tracing { get; private set; }

        protected BasePlugin() {}

        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            Context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            Tracing = (ITracingService)
                serviceProvider.GetService(typeof(ITracingService));

            var factory = (IOrganizationServiceFactory)
                serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            Service = factory.CreateOrganizationService(null);

            ExecuteInternal();
        }

        protected abstract void ExecuteInternal();
    }
}
