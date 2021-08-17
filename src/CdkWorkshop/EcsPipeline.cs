using Amazon.CDK;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.CodePipeline.Actions;
using Amazon.CDK.AWS.CodeBuild;
using Amazon.CDK.AWS.CodePipeline;

using System;
using System.Collections.Generic;

namespace CdkWorkshop
{
    public class EcsPipelineProps
    {
        // The function for which we want to count url hits
        public IRepository Repo { get; set; }
        public IBaseService ServiceIdentifier { get; set; }
        public string ContainerName { get; set; }
    }

    public class EcsPipeline : Construct
    {
        public PipelineProject GetStandardImageBuild(Construct stack, string containerName)
        {
            return new PipelineProject(stack, "buildProject", new PipelineProjectProps
            {
                BuildSpec = BuildSpec.FromObject(new Dictionary<string, object>
                 {
                     ["version"] = "0.2",
                     ["phases"] = new Dictionary<string, object>
                     {
                         ["install"] = new Dictionary<string, object>
                         {
                             ["commands"] = new string[]
                             {
                                 "apt-get install jq -y"
                             }
                         },
                         ["build"] = new Dictionary<string, object>
                         {
                             ["commands"] = new string[]
                             {
                                 "cd ${CODEBUILD_SRC_DIR}",
                                 $"ContainerName=\"{containerName}\"",
                                 "ImageURI=$(cat imageDetail.json | jq -r '.ImageURI') || exit 1",
                                 "printf '[{\"name\":\"CONTAINER_NAME\",\"imageUri\":\"IMAGE_URI\"}]' > imagedefinitions.json",
                                 "sed -i -e \"s|CONTAINER_NAME|$ContainerName|g\" imagedefinitions.json",
                                 "sed -i -e \"s|IMAGE_URI|$ImageURI|g\" imagedefinitions.json",
                                 "cat imagedefinitions.json"
                             }
                         }
                     },
                     ["artifacts"] = new Dictionary<string, object>
                     {
                         ["base-directory"] = "${CODEBUILD_SRC_DIR}",
                         ["files"] = new[]
                         {
                             "imagedefinitions.json",
                         }
                     }
                 }),
                Environment = new BuildEnvironment
                {
                    BuildImage = LinuxBuildImage.STANDARD_5_0
                }
            });
        }
        public EcsPipeline(Construct scope, string id, EcsPipelineProps props) : base(scope, id)
        {
            EcrSourceAction source = new EcrSourceAction(new EcrSourceActionProps{
                ActionName = "SourceFromECR",
                Output = new Artifact_("EcrArtifact"),
                Repository = props.Repo,
                ImageTag = "latest"
            });

            PipelineProject build = GetStandardImageBuild(this, props.ContainerName);
            Artifact_ buildOutput = new Artifact_("BuildOutput");

            new Pipeline(this, "PlatformPipeline", new PipelineProps
            {
                PipelineName = "Ecr-deploy",
                Stages = new[]
                {
                    new Amazon.CDK.AWS.CodePipeline.StageProps
                    {
                        StageName = "Source",
                        Actions = new IAction[] {source},
                    },
                    new Amazon.CDK.AWS.CodePipeline.StageProps
                    {
                        StageName = "Build",
                        Actions = new []
                        {
                            new CodeBuildAction(new CodeBuildActionProps
                            {
                                ActionName = "build",
                                Project = build,
                                Input = source.ActionProperties.Outputs[0],
                                Outputs = new [] { buildOutput }
                            }),
                        }
                    },
                    new Amazon.CDK.AWS.CodePipeline.StageProps
                    {
                        StageName = "Deploy",
                        Actions = new []
                        {
                            new EcsDeployAction(new EcsDeployActionProps
                            {
                                ActionName = "Deploy",
                                Service = props.ServiceIdentifier,
                                Input = buildOutput
                            }),
                        }
                    }
                }
            });
        }

    }
}