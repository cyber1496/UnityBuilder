#if !UNITY_2019 && UNITY_2018_4_OR_NEWER
#define COMPATIBILITY_UNITY_2018_4
#endif
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
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
                string outputPath = ConverPath(Path.GetFullPath(helper.OutputPath));
                SetUpForUnity2018_4(Path.GetDirectoryName(scriptPath), outputPath);
                int result = CallScript(scriptPath, out string message,
                    outputPath,
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
        [Conditional("COMPATIBILITY_UNITY_2018_4")]
        void SetUpForUnity2018_4(string scriptPath, string outputPath) {
            Debug.Log($"COMPATIBILITY_UNITY_2018_4 enable.");
            string srcRootPath = Path.Combine(scriptPath, "compatibility-for-2018");
            Replace(srcRootPath, outputPath, "build.gradle.src", (src) =>
                src.Replace("[PRODUCT_NAME]", PlayerSettings.productName)
            );
            Replace(srcRootPath, outputPath, "settings.gradle.src", (src) =>
                src.Replace("[PRODUCT_NAME]", PlayerSettings.productName)
            );
            Replace(srcRootPath, outputPath, "local.properties.src", (src) =>
                src.Replace("[ANDROID_SDK]", EditorPrefs.GetString("AndroidSdkRoot").Replace(":", "\\:"))
                   .Replace("[ANDROID_NDK]", EditorPrefs.GetString("AndroidNdkRootR16b").Replace(":", "\\:"))
            );
            string gradlePath = $"{outputPath}/{PlayerSettings.productName}/build.gradle";
            File.WriteAllText(gradlePath,
                File.ReadAllText(gradlePath).Replace(
                    "apply plugin: 'com.android.library'",
                    "apply plugin: 'com.android.application'"
                ));
        }
        [Conditional("COMPATIBILITY_UNITY_2018_4")]
        void Replace(string root, string output, string srcFileName, Func<string, string> process) {
            string srcPath = Path.Combine(root, srcFileName);
            string dstPath = srcPath.Replace(root, output).Replace(".src", "");
            File.WriteAllText(dstPath, process(File.ReadAllText(srcPath)));
        }
        string ConverPath(string path) {
#if UNITY_EDITOR_WIN
            path = path.Replace("/", "\\");
#endif
            return path;
        }
        int CallScript(string script, out string message, params string[] args) {
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
            public static string BuildToolJarFileName =>
#if COMPATIBILITY_UNITY_2018_4
                "bundletool-all-0.6.0.jar";
#else
                "bundletool-all-0.10.3.jar";
#endif
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
#if COMPATIBILITY_UNITY_2018_4
                    $"{PlayerSettings.productName}/build/outputs/bundle/{variant}/{PlayerSettings.productName}" :
                    $"{PlayerSettings.productName}/build/outputs/apk/{variant}/{PlayerSettings.productName}-{variant}";
#else
                    $"launcher/build/outputs/bundle/{variant}/launcher" :
                    $"launcher/build/outputs/apk/{variant}/launcher-{variant}";
#endif
        }
    }
}