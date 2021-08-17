using Amazon.CDK;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.CodePipeline.Actions;
using Amazon.CDK.AWS.ECR;

using System;
using System.Collections.Generic;

namespace CdkWorkshop
{
    public class DeployEcsStage : Stage
    {
        public List<IBaseService> Services { get; set; }
        public string[] Regions = { "eu-west-2", "us-east-1", "ap-southeast-1", "ap-southeast-2", "ca-central-1"};

        public DeployEcsStage(Construct scope, string id, string account, string ecrRepo, StageProps props = null) : base(scope, id, props)
        {
            RepositoryAttributes repositoryAttributes = new RepositoryAttributes();
            repositoryAttributes.RepositoryArn = ecrRepo;
            repositoryAttributes.RepositoryName = "rl-engine-repo";
            IRepository repo = Repository.FromRepositoryAttributes(this, "ecrRepo", repositoryAttributes);


            foreach (string region in Regions){
                EcsStack ecsStack = new EcsStack(this, $"ECS-{region}", repo, new StackProps {
                    Env = new Amazon.CDK.Environment {Region = region, Account = account}});
            }
        }
    }
}
