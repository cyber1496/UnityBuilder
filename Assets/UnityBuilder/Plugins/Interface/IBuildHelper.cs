using UnityEditor;

namespace UnityBuilder {
    public interface IBuildHelper {
        string RootPath { get; }
        string[] TargetScenes { get; }
        string OutputPath { get; }
        string OutputFile { get; }
        string OutputExt { get; }
        BuildTarget BuildTarget { get; }
        BuildTargetGroup BuildTargetGroup { get; }
        BuildOptions BuildOptions { get; }
    }
}