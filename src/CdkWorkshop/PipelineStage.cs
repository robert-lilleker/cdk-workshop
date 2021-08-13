using Amazon.CDK;
using Amazon.CDK.Pipelines;

namespace CdkWorkshop
{
    public class WorkshopPipelineStage : Stage
    {
        public string[] Regions = { "eu-west-2", "us-east-1", "ap-southeast-1", "ap-southeast-2", "ca-central-1"};
        // private void addDeployment(string region, CdkPipeline pipeline)
        // {



        //     deployStage.AddActions(new ShellScriptAction(new ShellScriptActionProps
        //     {
        //         ActionName = $"TestViewerEndpoint-{region}",
        //         UseOutputs = new Dictionary<string, StackOutput> {
        //             { "ENDPOINT_URL", pipeline.StackOutput(deploy.HCViewerUrl) }
        //         },
        //         Commands = new string[] {"curl -Ssf $ENDPOINT_URL"}
        //     }));
        //     deployStage.AddActions(new ShellScriptAction(new ShellScriptActionProps
        //     {
        //         ActionName = $"TestAPIGatewayEndpoint-{region}",
        //         UseOutputs = new Dictionary<string, StackOutput> {
        //             { "ENDPOINT_URL", pipeline.StackOutput(deploy.HCEndpoint) }
        //         },
        //         Commands = new string[] {
        //             "curl -Ssf $ENDPOINT_URL/",
        //             "curl -Ssf $ENDPOINT_URL/hello",
        //             "curl -Ssf $ENDPOINT_URL/test"
        //         }
        //     }));

        //}
        public WorkshopPipelineStage(Construct scope, string id, string account, StageProps props = null)
            : base(scope, id, props)
        {
            foreach (string region in Regions){
                new CdkWorkshopStack(this, $"WebService-{region}", new StackProps {
                    Env = new Amazon.CDK.Environment {Region = region, Account = account}});
            }
        }
    }
}