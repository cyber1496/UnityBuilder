using UnityEngine;
using UnityEditor;

namespace UnityBuilder {

    public enum EnvironmentType {
        Mac,
        WSL,
        Windows_NT
    }

    public interface IBuildHelperAsset {
        IBuildHelper GetBuildHelper();
    }

    public interface IBuildHelper {
        string RootPath { get; }
        string[] TargetScenes { get; }
        string OutputPath { get; }
        string OutputFile { get; }
        string OutputExt { get; }
        UnityEditor.BuildTarget BuildTarget { get; }
        UnityEditor.BuildTargetGroup BuildTargetGroup { get; }
        BuildOptions BuildOptions { get; }
        IBuildArguments BuildArguments { get; }
        bool IsBatchMode { get; }
        Scheme Scheme { get; }

        string GetReplacedPath(string path);
        EnvironmentType GetEnvironmentType();
        bool IsEnvironment(EnvironmentType environmentType);
    }
}