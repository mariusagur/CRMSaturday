using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace Dyn365PluginsAndWorkflows.Workflows
{
    public class PassContextToServiceEndpoint : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            var workflowContext = context.GetExtension<IWorkflowContext>();
            var service = context.GetExtension<IServiceEndpointNotificationService>();
            var response = service.Execute(ServiceEndpoint.Get(context), workflowContext);
            ReturnValue.Set(context, response);
        }

        [Input("ServiceEndpoint")]
        [ReferenceTarget("serviceendpoint")]
        [RequiredArgument]
        public InArgument<EntityReference> ServiceEndpoint { get; set; }

        [Output("Service Return Value")]
        public OutArgument<string> ReturnValue { get; set; }
    }
}
