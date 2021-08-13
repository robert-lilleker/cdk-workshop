using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;

namespace CdkWorkshop
{
    public class EcsStack : Stack
    {
        public IBaseService Service { get; set; }
        public EcsStack(Construct parent, string id, IStackProps props = null) : base(parent, id, props)
        {
            var vpc = new Vpc(this, "rl-test-vpc", new VpcProps
            {
                MaxAzs = 3 // Default is all AZs in region
            });

            var cluster = new Cluster(this, "rl-test-this-clust", new ClusterProps
            {
                Vpc = vpc
            });

            // Create a load-balanced Fargate service and make it public
            ApplicationLoadBalancedFargateService service = new ApplicationLoadBalancedFargateService(this, "MyFargateService",
                new ApplicationLoadBalancedFargateServiceProps
                {
                    Cluster = cluster,          // Required
                    DesiredCount = 6,           // Default is 1
                    TaskImageOptions = new ApplicationLoadBalancedTaskImageOptions
                    {
                        Image = ContainerImage.FromRegistry("amazon/amazon-ecs-sample")
                    },
                    MemoryLimitMiB = 2048,      // Default is 256
                    PublicLoadBalancer = true    // Default is false
                }
            );
            FargateServiceAttributes fargateAttributes = new FargateServiceAttributes();
            fargateAttributes.Cluster = cluster;
            fargateAttributes.ServiceName = service.Service.ServiceName;

            Service = FargateService.FromFargateServiceAttributes(
                this, "service", fargateAttributes);
        }
    }
}