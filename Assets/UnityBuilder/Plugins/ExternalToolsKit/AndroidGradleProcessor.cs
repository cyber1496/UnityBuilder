using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace UnityBuilder.ExternalToolKit {
    public sealed class AndroidGradleProcessor : IPostProcessor {
        public int PostOrder => 0;
        public void PostProcess(IBuildHelper helper) {
            Debug.Log("AndroidGradleProcessor.PostProcess");
            if (helper.BuildTarget != BuildTarget.Android) {
                return;
            }
            if (!helper.BuildOptions.HasFlag(BuildOptions.AcceptExternalModificationsToPlayer)) {
                return;
            }
#if UNITY_EDITOR_WIN
            string scriptName = "gradle-build.bat";
#else
            string scriptName = "gradle-build.sh";
#endif
            string scriptPath = ConverPath(Path.Combine(helper.RootPath, $"Assets/UnityBuilder/Plugins/ExternalToolsKit/AndroidGradleProcessor/{scriptName}"));
            string outputPath = ConverPath(Path.GetFullPath(helper.OutputPath));
            int result = CallScript(scriptPath, out string message, outputPath, EditorUserBuildSettings.development ? "debug/launcher-debug.apk" : "release/launcher-release.apk", helper.OutputExt);
            //message = Encoding.UTF8.GetString(Encoding.Default.GetBytes(message));
            if (result != 0) {
                throw new System.Exception(message);
            }
            else {
                Debug.Log(message);
            }
        }
        string ConverPath(string path) {
#if UNITY_EDITOR_WIN
            path = path.Replace("/","\\");
#endif
            return path;
        }
        public int CallScript(string script, out string message, params string[] args) {
            var p = new Process();
            string argstr = $"\"{script}\" \"{string.Join("\" \"", args)}\"";
#if UNITY_EDITOR_WIN
            p.StartInfo.FileName = Environment.GetEnvironmentVariable("ComSpec");
            p.StartInfo.Arguments = $"/c \"{argstr}\"";
#else
            p.StartInfo.FileName = "/bin/bash";
            p.StartInfo.Arguments = $"-c \"sh {argstr}\"";
#endif
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
    }
}