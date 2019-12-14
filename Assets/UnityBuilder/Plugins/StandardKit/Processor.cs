using UnityEditor;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;

namespace UnityBuilder.StandardKit {
    public class Processor : IProcessor {
        public BuildResult Process(IBuildHelper helper) {
            var report = BuildPipeline.BuildPlayer(helper.TargetScenes, helper.OutputPath, helper.BuildTarget, helper.BuildOptions);
            var summary = report.summary;
            Debug.Log($"result:{summary.result}, outputPath:{summary.outputPath}, totalTime:{summary.totalTime}");
            return summary.result;
        }
    }
}