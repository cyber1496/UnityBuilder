using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Injector;
using Debug = UnityEngine.Debug;
#if UNITY_EDITOR_OSX
using UnityEditor.iOS.Xcode;
#endif

namespace UnityBuilder {
    [CreateAssetMenu(menuName = "UnityBuilder/CCacheTaskAsset", fileName = "CCacheTaskAsset")]
    public sealed class CCacheTaskAsset : BuildTaskAsset {

        public override IBuildTask GetBuildTask(IBuildHelper helper)
            => new CCacheTask(helper, this);

        public sealed class CCacheTask : BuildTask {
            private readonly CCacheTaskAsset taskAsset;

            public CCacheTask(IBuildHelper helper, CCacheTaskAsset asset)
                : base(helper, asset)
            {
                this.taskAsset = asset;
            }
            public override int Version => 1;
            protected override ReturnCode onRun() {
#if UNITY_EDITOR_OSX
                string scriptFileName = "ccache.sh";
                string scriptSrcFilePath = Path.GetFullPath($"Packages/com.cyber1496.unitybuilder/Editor/Impl/Task/CCache/{scriptFileName}.src");
                string chmodScriptFilePath = Path.GetFullPath("Packages/com.cyber1496.unitybuilder/Editor/Impl/Task/CCache/chmod.sh");

                string xcodePath = EditorPrefs.GetString("UnityBuilder.StandardKit.iOS.XcodePath");
                string outputScriptPath = Path.Combine(helper.OutputPath, scriptFileName);
                if (File.Exists(outputScriptPath)) {
                    File.Delete(outputScriptPath);
                }
                File.WriteAllText(outputScriptPath, File.ReadAllText(scriptSrcFilePath).Replace("[XCODE_PATH]", xcodePath));

                ReturnCode returnCode = ReturnCode.Success;
                Utility.ExecuteScript(new ProcessRequest(
                    chmodScriptFilePath,
                    "Logs/chmod.log",
                    new string[] { "555", Path.GetFullPath(outputScriptPath) },
                    result =>
                    {
                        returnCode = result.ExitCode == 0 ? ReturnCode.Success : ReturnCode.Error;
                    }
                ));

                var pbxProjectPath = PBXProject.GetPBXProjectPath(helper.OutputPath);
                var pbxProject = new PBXProject();
                pbxProject.ReadFromString(File.ReadAllText(pbxProjectPath));

                var target = pbxProject.ProjectGuid();
                pbxProject.AddBuildProperty(target, "CC", Path.GetFullPath(outputScriptPath));
                pbxProject.SetBuildProperty(target, "LDPLUSPLUS", "$(DT_TOOLCHAIN_DIR)/usr/bin/clang++");

                File.WriteAllText(pbxProjectPath, pbxProject.WriteToString());

                return returnCode;
#else
                return ReturnCode.Success;
#endif
            }
        }
    }
}
