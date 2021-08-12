using Amazon.CDK;
using Amazon.CDK.Pipelines;

namespace CdkWorkshop
{
    public class WorkshopPipelineStage : Stage
    {
        public WorkshopPipelineStage(Construct scope, string id, StageProps props = null)
            : base(scope, id, props)
        {
            var service = new CdkWorkshopStack(this, "WebService");
        }
    }
}