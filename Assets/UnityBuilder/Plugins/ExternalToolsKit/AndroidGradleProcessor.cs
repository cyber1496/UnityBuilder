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
            string result = CallScript(scriptPath, Path.GetFullPath(helper.OutputPath), "debug/launcher-debug.apk", helper.OutputExt);
            Debug.Log(result);
        }
        public string CallScript(string script, params string[] args) {
            var p = new Process();
            p.StartInfo.FileName = "/bin/bash";
            p.StartInfo.Arguments = $"-c \"sh \"{script}\" \"{string.Join("\" \"", args)}\"\"";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            var output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();
            return output;
        }
    }
}