using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace UnityBuilder.StandardKit {
    public class BuildHelper : IBuildHelper {
        public string RootPath => Application.dataPath.Replace("Assets", "");
        public string[] TargetScenes => new string[] { };
        public string OutputPath {
            get {
                bool exportExternalProject = BuildOptions.HasFlag(BuildOptions.AcceptExternalModificationsToPlayer);
                return $"build/{BuildTarget}/{OutputFile}" + (exportExternalProject ? "" : $"{OutputExt}");
            }
        }
        public string OutputFile => PlayerSettings.productName;
        public string OutputExt {
            get {
                switch (BuildTarget) {
                    case BuildTarget.Android: {
                            return EditorUserBuildSettings.buildAppBundle ? ".aab" : ".apk";
                        }
                    case BuildTarget.iOS: return ".ipa";
                    case BuildTarget.StandaloneWindows:
                    case BuildTarget.StandaloneWindows64: return ".exe";
                    case BuildTarget.StandaloneOSX: return ".app";
                    default: return "";
                }
            }
        }
        public BuildTarget BuildTarget => EditorUserBuildSettings.activeBuildTarget;
        public BuildTargetGroup BuildTargetGroup => EditorUserBuildSettings.selectedBuildTargetGroup;
        public BuildOptions BuildOptions {
            get {
                var opt = BuildOptions.None;
                switch (BuildTarget) {
                    case BuildTarget.Android:
                    case BuildTarget.iOS: opt |= BuildOptions.AcceptExternalModificationsToPlayer; break;
                }
                return opt;
            }
        }
        public IBuildArguments BuildArguments { get; } = new BuildArguments();
        public bool IsBatchMode => BuildArguments.ContainsKey("-batchmode");
        public string GetReplacedPath(string path) {
            path = path.Replace("${PROJECT_ROOT}/", RootPath);
            path = path.Replace("${HOME}", Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            path = Path.GetFullPath(path);
            return path;
        }
    }
}