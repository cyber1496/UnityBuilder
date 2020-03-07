using System;
using System.Text;
using System.IO;
using System.Diagnostics;
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
            string outputPath = helper.OutputPath;
            string exportOptionPlistPath = CreateExportOption(outputPath);
            int result = CallScript(scriptPath, out string message,
                outputPath,
                EditorUserBuildSettings.development ? "Debug" : "Release",
                exportOptionPlistPath
            );
            if (result != 0) {
                throw new Exception(message);
            }
            else {
                if (message.Length >= ushort.MaxValue) {
                    string logFilePath = "Logs/iOS/xcode-build.log";
                    File.WriteAllText(logFilePath, message);
                    Debug.Log($"log dump to {logFilePath}.");
                }
                else {
                    Debug.Log(message);
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
        int CallScript(string script, out string message, params string[] args) {
            var p = new Process();
            p.StartInfo.FileName = XcodeEnvironment.ProcessorFileName;
            p.StartInfo.Arguments = XcodeEnvironment.GetArgumentsForProcessor(script, args);
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            p.Start();
            message = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            int exitcode = p.ExitCode;
            p.Close();
            return exitcode;
        }
        static class XcodeEnvironment {
            static string ScriptFileName
                => "xcode-build.sh";
            public static string ScriptFilePath
                => $"Assets/UnityBuilder/Plugins/ExternalToolsKit/XcodeProcessor/{ScriptFileName}";
            public static string ProcessorFileName => "/bin/bash";
            public static string GetArgumentsForProcessor(string script, string[] args) {
                string argstr = $"\"{script}\" \"{string.Join("\" \"", args)}\"";
                return $"-c \"sh {argstr}\"";
            }
        }
#endif
    }
}