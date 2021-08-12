using Amazon.CDK;
using Amazon.CDK.Pipelines;

namespace CdkWorkshop
{
    public class WorkshopPipelineStage : Stage
    {
        public readonly CfnOutput HCViewerUrl;
        public readonly CfnOutput HCEndpoint;
        public WorkshopPipelineStage(Construct scope, string id, string region, StageProps props = null)
            : base(scope, id, props)
        {
            var service = new CdkWorkshopStack(this, "WebService", new StackProps { Env = new Amazon.CDK.Environment {Region = region}});
            this.HCEndpoint = service.HCEndpoint;
            this.HCViewerUrl = service.HCViewerUrl;
        }
    }
}