using System;
using System.IO;
using UnityEditor;
using Debug = UnityEngine.Debug;
#if UNITY_EDITOR_OSX
using UnityEditor.iOS.Xcode;
#endif

namespace UnityBuilder.ExternalToolKit {
    public sealed class CCacheProcessor : IPostProcessor {
        public int PostOrder => 9;
        public void PostProcess(IBuildHelper helper) {
            if (helper.BuildTarget != BuildTarget.iOS) {
                return;
            }

#if UNITY_EDITOR_OSX
            Environment.SetupScript(helper);
#endif
        }
#if UNITY_EDITOR_OSX
        static class Environment {
            static string XcodePath
                => "/Applications/Xcode13.2.1.app";
            static string ScriptFileName
                => "ccache.sh";
            static string ScriptSrcFilePath
                => $"Packages/com.cyber1496.unitybuilder/Editor/ExternalToolsKit/XcodeProcessor/{ScriptFileName}.src";
            static string ChmodScriptFilePath
                => "Packages/com.cyber1496.unitybuilder/Editor/ExternalToolsKit/CCacheProcessor/chmod.sh";
            public static void SetupScript(IBuildHelper helper) {
                string inputScriptPath = Path.Combine(helper.RootPath, ScriptSrcFilePath);
                string outputScriptPath = Path.Combine(helper.OutputPath, ScriptFileName);
                File.WriteAllText(outputScriptPath, File.ReadAllText(inputScriptPath).Replace("[XCODE_PATH]", XcodePath));

                Utility.ExecuteScript(new ProcessRequest(
                    ChmodScriptFilePath,
                    "Logs/chmod.log",
                    new string[] { "555", Path.GetFullPath(outputScriptPath) }
                ));

                var pbxProjectPath = PBXProject.GetPBXProjectPath(helper.OutputPath);
                var pbxProject = new PBXProject();
                pbxProject.ReadFromString(File.ReadAllText(pbxProjectPath));

                var target = pbxProject.ProjectGuid();
                pbxProject.AddBuildProperty(target, "CC", Path.GetFullPath(outputScriptPath));
                pbxProject.SetBuildProperty(target, "LDPLUSPLUS", "$(DT_TOOLCHAIN_DIR)/usr/bin/clang++");

                File.WriteAllText(pbxProjectPath, pbxProject.WriteToString());
            }
        }
#endif
    }
}