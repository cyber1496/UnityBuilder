using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Injector;


namespace UnityBuilder
{
    [CreateAssetMenu(menuName = "UnityBuilder/BuildPlayerTaskAsset", fileName = "BuildPlayerTaskAsset")]
    public sealed class BuildPlayerTaskAsset : BuildTaskAsset
    {
        public override IBuildTask GetBuildTask(IBuildHelper helper)
            => new BuildPlayerTask(helper, this);

        public sealed class BuildPlayerTask : BuildTask
        {
            private readonly BuildPlayerTaskAsset taskAsset;

            public BuildPlayerTask(IBuildHelper helper, BuildPlayerTaskAsset asset)
                : base(helper, asset)
            {
                this.taskAsset = asset;
            }

            public override int Version => 1;

            protected override ReturnCode onRun()
            {
                var report = BuildPipeline.BuildPlayer(helper.TargetScenes, helper.OutputPath, helper.BuildTarget,
                    helper.BuildOptions);
                var summary = report.summary;
                Debug.Log($"result:{summary.result}, outputPath:{summary.outputPath}, totalTime:{summary.totalTime}");
                return summary.result == BuildResult.Succeeded ? ReturnCode.Success : ReturnCode.Error;
            }
        }
    }
}