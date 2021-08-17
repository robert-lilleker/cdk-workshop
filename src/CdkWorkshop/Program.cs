using Amazon.CDK;

namespace CdkWorkshop
{
    sealed class Program
    {

        public static void Main(string[] args)
        {
            Environment makeEnv(string account, string region)
            {
                return new Amazon.CDK.Environment
                {
                    Account = account,
                    Region = region
                };
            }
            var app = new App();
            Environment env = makeEnv(account: "442608252338", region: "eu-west-2");
            new WorkshopPipelineStack(app, "GitWorkshopPipelineStack", new StackProps { Env=env});

            app.Synth();
        }
    }
}
