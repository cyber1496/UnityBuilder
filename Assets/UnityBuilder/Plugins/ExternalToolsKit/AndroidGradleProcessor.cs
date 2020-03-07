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
            {
                string variant = GradleEnvironment.GetBuildVariant(EditorUserBuildSettings.development);
                string scriptPath = ConverPath(Path.Combine(helper.RootPath, GradleEnvironment.GradleScriptFilePath));
                int result = CallScript(scriptPath, out string message,
                    ConverPath(Path.GetFullPath(helper.OutputPath)),
                    ConverPath(GradleEnvironment.GetOutputFilePath(variant)),
                    helper.OutputExt,
                    variant,
                    EditorUserBuildSettings.buildAppBundle.ToString()
                );
                if (result != 0) {
                    throw new System.Exception(message);
                }
                else {
                    Debug.Log(message);
                }
            }
            if (EditorUserBuildSettings.buildAppBundle) {
                string scriptPath = ConverPath(Path.Combine(helper.RootPath, GradleEnvironment.BuildToolScriptFilePath));
                int result = CallScript(scriptPath, out string message,
                    ConverPath(GradleEnvironment.BuildToolJarPath),
                    helper.OutputPath + helper.OutputExt,
                    helper.OutputPath + ".apks",
                    PlayerSettings.Android.keystoreName,
                    PlayerSettings.Android.keystorePass,
                    PlayerSettings.Android.keyaliasName,
                    PlayerSettings.Android.keyaliasPass
                );
                if (result != 0) {
                    throw new System.Exception(message);
                }
                else {
                    Debug.Log(message);
                }
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
            static string GradleScriptFileName =>
#if UNITY_EDITOR_WIN
                "gradle-build.bat";
#else
                "gradle-build.sh";
#endif
            public static string GradleScriptFilePath =>
                $"Assets/UnityBuilder/Plugins/ExternalToolsKit/AndroidGradleProcessor/{GradleScriptFileName}";
            static string BuildToolScriptFileName =>
#if UNITY_EDITOR_WIN
                    "buildtool-build-apks.bat";
#else
                    "buildtool-build-apks.sh";
#endif
            public static string BuildToolJarFileName => "bundletool-all-0.10.3.jar";
            public static string BuildToolJarPath =>
#if UNITY_EDITOR_WIN
                Path.Combine(Path.GetDirectoryName(EditorApplication.applicationPath), $"Data/PlaybackEngines/AndroidPlayer/Tools/{BuildToolJarFileName}");
#else
                Path.Combine(Path.GetDirectoryName(EditorApplication.applicationPath), $"PlaybackEngines/AndroidPlayer/Tools/{BuildToolJarFileName}");
#endif

            public static string BuildToolScriptFilePath =>
                $"Assets/UnityBuilder/Plugins/ExternalToolsKit/AndroidGradleProcessor/{BuildToolScriptFileName}";
            public static string ProcessorFileName =>
#if UNITY_EDITOR_WIN
                Environment.GetEnvironmentVariable("ComSpec");
#else
                "/bin/bash";
#endif
            public static string GetArgumentsForProcessor(string script, string[] args) {
                string argstr = $"\"{script}\" \"{string.Join("\" \"", args)}\"";
#if UNITY_EDITOR_WIN
                return $"/c \"{argstr}\"";
#else
                return $"-c \"sh {argstr}\"";
#endif
            }
            public static string GetBuildVariant(bool isDevelopment) =>
                isDevelopment ? "debug" : "release";
            public static string GetOutputFilePath(string variant) =>
                EditorUserBuildSettings.buildAppBundle ?
                    $"launcher/build/outputs/bundle/{variant}/launcher" :
                    $"launcher/build/outputs/apk/{variant}/launcher-{variant}";
        }
    }
}