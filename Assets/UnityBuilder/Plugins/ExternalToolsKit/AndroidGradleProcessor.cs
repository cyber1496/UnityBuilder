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
            string scriptPath = Path.Combine(helper.RootPath, "Assets/UnityBuilder/Plugins/ExternalToolsKit/AndroidGradleProcessor/gradle-build.sh");
            int result = CallScript(scriptPath, out string message, Path.GetFullPath(helper.OutputPath), "debug/launcher-debug.apk", helper.OutputExt);
            if (result != 0) {
                throw new System.Exception(message);
            }
        }
        public int CallScript(string script, out string message, params string[] args) {
            var p = new Process();
            p.StartInfo.FileName = "/bin/bash";
            p.StartInfo.Arguments = $"-c \"sh \"{script}\" \"{string.Join("\" \"", args)}\"\"";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            message = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            int exitcode = p.ExitCode;
            p.Close();
            return exitcode;
        }
    }
}