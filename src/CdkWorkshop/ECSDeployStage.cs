using Amazon.CDK;
using Amazon.CDK.AWS.CodePipeline.Actions;
using Amazon.CDK.AWS.ECR;

namespace CdkWorkshop
{
    public class DeployEcsStage : Stage
    {
        public string[] Regions = { "eu-west-2", "us-east-1", "ap-southeast-1", "ap-southeast-2", "ca-central-1"};

        public DeployEcsStage(Construct scope, string id, string account, IRepository ecrRepo, StageProps props = null) : base(scope, id, props)
        {
            foreach (string region in Regions){
                EcsStack ecsStack = new EcsStack(this, $"ECS-{region}", ecrRepo, new StackProps {
                    Env = new Amazon.CDK.Environment {Region = region, Account = account}});
            }
        }
    }
}
