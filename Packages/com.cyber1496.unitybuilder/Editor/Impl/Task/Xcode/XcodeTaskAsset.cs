using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Injector;

#if UNITY_EDITOR_OSX
using UnityEditor.iOS.Xcode;
#endif

namespace UnityBuilder {
    [CreateAssetMenu(menuName = "UnityBuilder/XcodeTaskAsset", fileName = "XcodeTaskAsset")]
    public sealed class XcodeTaskAsset : BuildTaskAsset {
        
        public override IBuildTask GetBuildTask(IBuildHelper helper)
            => new XcodeTask(helper, this);

        public sealed class XcodeTask : BuildTask {
            private readonly XcodeTaskAsset taskAsset;
            public XcodeTask(IBuildHelper helper, XcodeTaskAsset asset)
                : base(helper, asset)
            {
                this.taskAsset = asset;
            }
            public override int Version => 1;
            protected override ReturnCode onRun() {
                try {
#if UNITY_EDITOR_OSX
                    string pbxPath = PBXProject.GetPBXProjectPath(helper.OutputPath);
                    PBXProject pbx = new PBXProject();
                    pbx.ReadFromString(File.ReadAllText(pbxPath));
                    string target = pbx.GetUnityMainTargetGuid();

                    pbx.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
                    target = pbx.GetUnityFrameworkTargetGuid();
                    pbx.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
                    File.WriteAllText (pbxPath, pbx.WriteToString());

                    string scriptFileName = "xcode-build.sh";
                    string scriptFilePath = Path.GetFullPath($"Packages/com.cyber1496.unitybuilder/Editor/Impl/Task/Xcode/{scriptFileName}");

                    string CreateExportOption(string outputPath) {
                        string path = $"{outputPath}/ExportOptions.plist";
                        var plist = new PlistDocument();
                        plist.root.SetString("method", "development");
                        plist.root.SetString("teamID", PlayerSettings.iOS.appleDeveloperTeamID);
                        plist.WriteToFile(path);
                        return path;
                    }

                    string xcodePath = EditorPrefs.GetString("UnityBuilder.StandardKit.iOS.XcodePath");
                    string scriptPath = scriptFilePath;
                    string logPath = $"Logs/{helper.BuildTarget}/{Path.GetFileName(scriptPath)}.log";
                    string outputPath = helper.OutputPath;
                    string exportOptionPlistPath = CreateExportOption(outputPath);
                    var chunk = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS).Split('.');
                    Utility.ExecuteScript(new ProcessRequest(
                        scriptPath,
                        logPath,
                        new string[] {
                            outputPath,
                            xcodePath,
                            EditorUserBuildSettings.development ? "Debug" : "Release",
                            exportOptionPlistPath
                        },
                        (result) => {
                            if (result.ExitCode != 0) {
                                throw new Exception($"{scriptPath} exit is {result.ExitCode}.");
                            }
                        }
                    ));

                    return ReturnCode.Success;
#else
                    return ReturnCode.Success;
#endif
                }
                catch (Exception ex) {
                    Debug.LogException(ex);
                    if (helper.IsBatchMode) {
                        EditorApplication.Exit(1);
                    }

                    return ReturnCode.Error;
                }
            }
        }
    }
}