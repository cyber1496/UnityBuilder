using UnityEditor;

namespace UnityBuilder.StandardKit {
    public class BuildHelper : IBuildHelper {
        public string[] TargetScenes => new string[] { };
        public string OutputPath => $"build/{OutputFile}{OutputExt}";
        public string OutputFile => BuildTarget.ToString();
        public string OutputExt => ".apk";
        public BuildTarget BuildTarget => BuildTarget.Android;
        public BuildOptions BuildOptions => BuildOptions.None;
    }
}