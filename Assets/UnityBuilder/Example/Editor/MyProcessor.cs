using UnityBuilder;
using UnityEditor;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;

public class MyProcessor : IProcessor {
    public BuildResult Process(IBuildHelper helper) {
        Debug.Log("MyProcessor.Process");
        var report = BuildPipeline.BuildPlayer(helper.TargetScenes, helper.OutputPath, helper.BuildTarget, helper.BuildOptions);
        var summary = report.summary;
        Debug.Log($"result:{summary.result}, outputPath:{summary.outputPath}, totalTime:{summary.totalTime}");
        return summary.result;
    }
}