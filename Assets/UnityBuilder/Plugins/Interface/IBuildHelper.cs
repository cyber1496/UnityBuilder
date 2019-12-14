using UnityEditor;

namespace UnityBuilder {
    public interface IBuildHelper {
        string[] TargetScenes { get; }
        string OutputPath { get; }
        string OutputFile { get; }
        string OutputExt { get; }
        BuildTarget BuildTarget { get; }
        BuildOptions BuildOptions { get; }
    }
}