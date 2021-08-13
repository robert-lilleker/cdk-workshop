using Amazon.CDK;
using Amazon.CDK.Pipelines;

namespace CdkWorkshop
{
    public class DeployEcsStage : Stage
    {
        public string[] Regions = { "eu-west-2", "us-east-1", "ap-southeast-1", "ap-southeast-2", "ca-central-1"};
        public DeployEcsStage(Construct scope, string id, string account, StageProps props = null) : base(scope, id, props)
        {
            foreach (string region in Regions){
                new CdkWorkshopStack(this, $"ECS-{region}", new StackProps {
                    Env = new Amazon.CDK.Environment {Region = region, Account = account}});
            }
        }
    }
}
