#if !UNITY_2019_1_OR_NEWER
#define COMPATIBILITY_UNITY_2018_4
#endif
using System;
using System.Diagnostics;
using System.IO;
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
            if (!EditorUserBuildSettings.exportAsGoogleAndroidProject) {
                return;
            }
            string variant = GradleEnvironment.GetBuildVariant(EditorUserBuildSettings.development);
            string gradleScriptPath = Utility.ConvertPath(Path.Combine(helper.RootPath, GradleEnvironment.GradleScriptFilePath));
            string gradleLogPath = $"Logs/{helper.BuildTarget}/{Path.GetFileName(gradleScriptPath)}.log";
            string outputPath = Utility.ConvertPath(Path.GetFullPath(helper.OutputPath));
            SetUpForUnity2018_4(Path.GetDirectoryName(gradleScriptPath), outputPath);
            try {
                Utility.ExecuteScript(new ProcessRequest(
                    gradleScriptPath,
                    gradleLogPath,
                    new string[] {
                        outputPath,
                        Utility.ConvertPath(GradleEnvironment.GetOutputFilePath(variant)),
                        helper.OutputExt,
                        variant,
                        EditorUserBuildSettings.buildAppBundle.ToString()
                    },
                    (gradleResult) => {
                        if (gradleResult.ExitCode != 0) {
                            throw new Exception($"{gradleScriptPath} exit is {gradleResult.ExitCode}.");
                        }
                        if (EditorUserBuildSettings.buildAppBundle) {
                            string buildToolScriptPath = Utility.ConvertPath(Path.Combine(helper.RootPath, GradleEnvironment.BuildToolScriptFilePath));
                            string buildToolLogPath = $"Logs/{helper.BuildTarget}/{Path.GetFileName(buildToolScriptPath)}.log";
                            Utility.ExecuteScript(new ProcessRequest(
                                buildToolScriptPath,
                                buildToolLogPath,
                                new string[] {
                                    Utility.ConvertPath(GradleEnvironment.BuildToolJarPath),
                                    helper.OutputPath + helper.OutputExt,
                                    helper.OutputPath + ".apks",
                                    PlayerSettings.Android.keystoreName,
                                    PlayerSettings.Android.keystorePass,
                                    PlayerSettings.Android.keyaliasName,
                                    PlayerSettings.Android.keyaliasPass
                                },
                                (boldToolResult) => {
                                    if (boldToolResult.ExitCode != 0) {
                                        throw new Exception($"{buildToolScriptPath} exit is {boldToolResult.ExitCode}.");
                                    }
                                }
                            ));
                        }
                    }
                ));
            }
            catch (Exception ex) {
                Debug.LogException(ex);
                if (helper.IsBatchMode) {
                    EditorApplication.Exit(1);
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
        static class GradleEnvironment {
            static string GradleScriptFileName =>
#if UNITY_EDITOR_WIN
                "gradle-build.bat";
#else
                "gradle-build.sh";
#endif
            public static string GradleScriptFilePath =>
                $"Packages/com.cyber1496.unitybuilder/Editor/ExternalToolsKit/AndroidGradleProcessor/{GradleScriptFileName}";
            public static string BuildToolJarFileName =>
#if COMPATIBILITY_UNITY_2018_4
                "bundletool-all-0.6.0.jar";
#else
                "bundletool-all-1.6.0.jar";
#endif
            public static string BuildToolJarPath =>
#if UNITY_EDITOR_WIN
                Path.Combine(Path.GetDirectoryName(EditorApplication.applicationPath), $"Data/PlaybackEngines/AndroidPlayer/Tools/{BuildToolJarFileName}");
#else
                Path.Combine(Path.GetDirectoryName(EditorApplication.applicationPath), $"PlaybackEngines/AndroidPlayer/Tools/{BuildToolJarFileName}");
#endif
            static string BuildToolScriptFileName =>
#if UNITY_EDITOR_WIN
                    "buildtool-build-apks.bat";
#else
                    "buildtool-build-apks.sh";
#endif
            public static string BuildToolScriptFilePath =>
                $"Packages/com.cyber1496.unitybuilder/Editor/ExternalToolsKit/AndroidGradleProcessor/{BuildToolScriptFileName}";
            public static string GetBuildVariant(bool isDevelopment) =>
                isDevelopment ? "debug" : "release";
            public static string GetOutputFilePath(string variant) =>
                EditorUserBuildSettings.buildAppBundle ?
#if COMPATIBILITY_UNITY_2018_4
                    $"{PlayerSettings.productName}/build/outputs/bundle/{variant}/{PlayerSettings.productName}" :
                    $"{PlayerSettings.productName}/build/outputs/apk/{variant}/{PlayerSettings.productName}-{variant}";
#elif UNITY_2020_1_OR_NEWER
                    $"launcher/build/outputs/bundle/{variant}/launcher-{variant}" :
                    $"launcher/build/outputs/apk/{variant}/launcher-{variant}";
#else
                    $"launcher/build/outputs/bundle/{variant}/launcher" :
                    $"launcher/build/outputs/apk/{variant}/launcher-{variant}";
#endif
        }
    }
}