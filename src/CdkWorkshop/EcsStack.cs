using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.CodePipeline;
using Amazon.CDK.AWS.CodeBuild;
using Amazon.CDK.AWS.CodePipeline.Actions;

namespace CdkWorkshop
{
    public class EcsStack : Stack
    {
        public IBaseService serviceIdentifier;
        public EcsStack(Construct parent, string id, string ecrRepo, IStackProps props = null) : base(parent, id, props)
        {
            var vpc = new Vpc(this, "rl-test-vpc", new VpcProps
            {
                MaxAzs = 3 // Default is all AZs in region
            });

            var cluster = new Cluster(this, "rl-test-this-clust", new ClusterProps
            {
                Vpc = vpc
            });
            RepositoryAttributes repositoryAttributes = new RepositoryAttributes();
            repositoryAttributes.RepositoryArn = ecrRepo;
            repositoryAttributes.RepositoryName = "rl-engine-repo";
            IRepository repo = Repository.FromRepositoryAttributes(this, "ecrRepo", repositoryAttributes);
            // Create a load-balanced Fargate service and make it public
            ApplicationLoadBalancedFargateService service = new ApplicationLoadBalancedFargateService(this, "MyFargateService",
                new ApplicationLoadBalancedFargateServiceProps
                {
                    Cluster = cluster,          // Required
                    DesiredCount = 1,           // Default is 1
                    TaskImageOptions = new ApplicationLoadBalancedTaskImageOptions
                    {
                        Image = ContainerImage.FromEcrRepository(repo, "latest")
                    },
                    MemoryLimitMiB = 512,      // Default is 256
                    PublicLoadBalancer = true    // Default is false
                }
            );
            FargateServiceAttributes fargateAttributes = new FargateServiceAttributes();
            fargateAttributes.Cluster = cluster;
            fargateAttributes.ServiceName = service.Service.ServiceName;
            serviceIdentifier = FargateService.FromFargateServiceAttributes(
                this, "service", fargateAttributes);

            var helloWithCounter = new EcsPipeline(this, "EcsPipeline", new EcsPipelineProps
            {
                Repo = repo,
                ServiceIdentifier = serviceIdentifier,
                ContainerName = service.TaskDefinition.DefaultContainer.ContainerName
            });





        }
    }
}