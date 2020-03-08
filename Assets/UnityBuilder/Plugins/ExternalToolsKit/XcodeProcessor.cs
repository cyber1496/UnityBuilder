using System;
using System.IO;
using UnityEditor;
using Debug = UnityEngine.Debug;
using UnityEditor.iOS.Xcode;

namespace UnityBuilder.ExternalToolKit {
    public sealed class XcodeProcessor : IPostProcessor {
        public int PostOrder => 10;
        public void PostProcess(IBuildHelper helper) {
            Debug.Log("XcodeProcessor.PostProcess");
            if (helper.BuildTarget != BuildTarget.iOS) {
                return;
            }
            if (!helper.BuildOptions.HasFlag(BuildOptions.AcceptExternalModificationsToPlayer)) {
                return;
            }

#if UNITY_EDITOR_OSX
            string scriptPath = Path.Combine(helper.RootPath, XcodeEnvironment.ScriptFilePath);
            string logPath = $"Logs/{helper.BuildTarget}/{Path.GetFileName(scriptPath)}.log";
            string outputPath = helper.OutputPath;
            string exportOptionPlistPath = CreateExportOption(outputPath);
            try {
                Utility.ExecuteScript(new ProcessRequest(
                    scriptPath,
                    logPath,
                    new string[] {
                        outputPath,
                        EditorUserBuildSettings.development ? "Debug" : "Release",
                        exportOptionPlistPath
                    },
                    (result) => {
                        if (result.ExitCode != 0) {
                            throw new Exception($"{scriptPath} exit is {result.ExitCode}.");
                        }
                    }
                ));
            }
            catch (Exception ex) {
                Debug.LogException(ex);
                if (helper.IsBatchMode) {
                    EditorApplication.Exit(1);
                }
            }
#endif
        }
#if UNITY_EDITOR_OSX
        string CreateExportOption(string outputPath) {
            string path = $"{outputPath}/ExportOptions.plist";
            var plist = new PlistDocument();
            plist.root.SetString("method", "development");
            plist.root.SetString("teamID", PlayerSettings.iOS.appleDeveloperTeamID);
            plist.WriteToFile(path);
            return path;
        }
        static class XcodeEnvironment {
            static string ScriptFileName
                => "xcode-build.sh";
            public static string ScriptFilePath
                => $"Assets/UnityBuilder/Plugins/ExternalToolsKit/XcodeProcessor/{ScriptFileName}";
        }
#endif
    }
}