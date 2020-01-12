using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace UnityBuilder.ExternalToolKit {
    public sealed class AndroidGradleProcessor : IPostProcessor {
        public int PostOrder => 10;
        public void PostProcess(IBuildHelper helper) {
            Debug.Log("AndroidGradleProcessor.PostProcess");
            if (helper.BuildTarget != BuildTarget.Android) {
                return;
            }
            if (!helper.BuildOptions.HasFlag(BuildOptions.AcceptExternalModificationsToPlayer)) {
                return;
            }
            string scriptPath = ConverPath(Path.Combine(helper.RootPath, GradleEnvironment.ScriptFilePath));
            string outputPath = ConverPath(Path.GetFullPath(helper.OutputPath));
            int result = CallScript(scriptPath, out string message, outputPath, GradleEnvironment.GetDefaultAPKFileName(EditorUserBuildSettings.development), helper.OutputExt);
            if (result != 0) {
                throw new System.Exception(message);
            }
            else {
                Debug.Log(message);
            }
        }
        string ConverPath(string path) {
#if UNITY_EDITOR_WIN
            path = path.Replace("/", "\\");
#endif
            return path;
        }
        public int CallScript(string script, out string message, params string[] args) {
            var p = new Process();
            p.StartInfo.FileName = GradleEnvironment.ProcessorFileName;
            p.StartInfo.Arguments = GradleEnvironment.GetArgumentsForProcessor(script, args);
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
        static class GradleEnvironment {
            static string ScriptFileName {
                get {
#if UNITY_EDITOR_WIN
                    return "gradle-build.bat";
#else
                    return "gradle-build.sh";
#endif
                }
            }
            public static string ScriptFilePath {
                get {
                    return $"Assets/UnityBuilder/Plugins/ExternalToolsKit/AndroidGradleProcessor/{ScriptFileName}";
                }
            }
            public static string ProcessorFileName {
                get {
#if UNITY_EDITOR_WIN
                    return Environment.GetEnvironmentVariable("ComSpec");
#else
                    return "/bin/bash";
#endif
                }
            }
            public static string GetArgumentsForProcessor(string script, string[] args) {
                string argstr = $"\"{script}\" \"{string.Join("\" \"", args)}\"";
#if UNITY_EDITOR_WIN
                return $"/c \"{argstr}\"";
#else
                return $"-c \"sh {argstr}\"";
#endif
            }
            public static string GetDefaultAPKFileName(bool isDevelopment) {
                return isDevelopment ? "debug/launcher-debug.apk" : "release/launcher-release.apk";
            }
        }
    }
}