using Amazon.CDK;
using Amazon.CDK.AWS.CodeCommit;
using Amazon.CDK.AWS.CodePipeline;
using Amazon.CDK.AWS.CodePipeline.Actions;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.Pipelines;
using System.Collections.Generic;


namespace CdkWorkshop
{
    public class WorkshopPipelineStack : Stack
    {
        public string[] Regions = { "eu-west-2", "us-east-1", "ap-southeast-1", "ap-southeast-2", "ca-central-1"};


        public WorkshopPipelineStack(Construct parent, string id, IStackProps props = null) : base(parent, id, props)
        {
            //ecr repo
            Amazon.CDK.AWS.ECR.IRepository ecrRepo = new Amazon.CDK.AWS.ECR.Repository(this, "Engine-Repo", new Amazon.CDK.AWS.ECR.RepositoryProps {
                ImageScanOnPush = true,
                RepositoryName = "rl-engine-repo"
            });
            ecrRepo.GrantPull(new AccountPrincipal("689529395349"));

            // Defines the artifact representing the sourcecode
            var sourceArtifact = new Artifact_();
            // Defines the artifact representing the cloud assembly
            // (cloudformation template + all other assets)
            var cloudAssemblyArtifact = new Artifact_();

            // The basic pipeline declaration. This sets the initial structure
            // of our pipeline
            var pipeline = new CdkPipeline(this, "Pipeline", new CdkPipelineProps
            {
                PipelineName = "GitWorkshopPipeline",
                CloudAssemblyArtifact = cloudAssemblyArtifact,

                SourceAction = new CodeStarConnectionsSourceAction ( new CodeStarConnectionsSourceActionProps {
                ActionName = "Source",
                ConnectionArn = "arn:aws:codestar-connections:eu-west-2:093652424831:connection/809671ca-a7ca-4ac6-ae5d-e823830175be",
                Output = sourceArtifact,
                Owner = "robert-lilleker",
                Repo = "cdk-workshop",
                Branch = "main"
                }),
                // Builds our source code outlined above into a could assembly artifact
                SynthAction = SimpleSynthAction.StandardNpmSynth(new StandardNpmSynthOptions
                {
                    SourceArtifact = sourceArtifact,  // Where to get source code to build
                    CloudAssemblyArtifact = cloudAssemblyArtifact,  // Where to place built source

                    InstallCommand = "npm install -g aws-cdk",
                    BuildCommand = "dotnet build src", // Language-specific build cmd
                    SynthCommand = "cdk synth"
                })
            });
            // var deployDev = new WorkshopPipelineStage(this, "Deploy-dev", "442608252338");
            // pipeline.AddApplicationStage(deployDev);
            // var deployProd = new WorkshopPipelineStage(this, "Deploy-prod", "689529395349");
            // pipeline.AddApplicationStage(deployProd);

            var EcsDev = new DeployEcsStage(this, "Ecs-dev", "442608252338", ecrRepo);
            pipeline.AddApplicationStage(EcsDev);
            // var EcsProd = new DeployEcsStage(this, "Ecs-prod", "689529395349", ecrRepo);
            // pipeline.AddApplicationStage(EcsProd);
        }
    }
}
