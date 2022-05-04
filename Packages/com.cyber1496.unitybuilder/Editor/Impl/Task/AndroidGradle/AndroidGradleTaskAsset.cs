using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Injector;


namespace UnityBuilder
{
    [CreateAssetMenu(menuName = "UnityBuilder/AndroidGradleTaskAsset", fileName = "AndroidGradleTaskAsset")]
    public sealed class AndroidGradleTaskAsset : BuildTaskAsset
    {

        public override IBuildTask GetBuildTask(IBuildHelper helper)
            => new AndroidGradleTask(helper, this);

        public sealed class AndroidGradleTask : BuildTask
        {
            private readonly AndroidGradleTaskAsset taskAsset;

            public AndroidGradleTask(IBuildHelper helper, AndroidGradleTaskAsset asset)
                : base(helper, asset)
            {
                this.taskAsset = asset;
            }

            public override int Version => 1;

            protected override ReturnCode onRun()
            {
                if (!EditorUserBuildSettings.exportAsGoogleAndroidProject)
                {
                    return ReturnCode.Success;
                }

                string variant = GradleEnvironment.GetBuildVariant(EditorUserBuildSettings.development);
                string gradleScriptPath = GradleEnvironment.GradleScriptFilePath;
                string gradleLogPath = $"Logs/{helper.BuildTarget}/{Path.GetFileName(gradleScriptPath)}.log";
                string outputPath = Utility.ConvertPath(Path.GetFullPath(helper.OutputPath));
                try
                {
                    Utility.ExecuteScript(new ProcessRequest(
                        gradleScriptPath,
                        gradleLogPath,
                        new string[]
                        {
                            outputPath,
                            Utility.ConvertPath(GradleEnvironment.GetOutputFilePath(variant)),
                            helper.OutputExt,
                            variant,
                            EditorUserBuildSettings.buildAppBundle.ToString()
                        },
                        (gradleResult) =>
                        {
                            if (gradleResult.ExitCode != 0)
                            {
                                throw new Exception($"{gradleScriptPath} exit is {gradleResult.ExitCode}.");
                            }

                            if (EditorUserBuildSettings.buildAppBundle)
                            {
                                string buildToolScriptPath = GradleEnvironment.BuildToolScriptFilePath;
                                string buildToolLogPath =
                                    $"Logs/{helper.BuildTarget}/{Path.GetFileName(buildToolScriptPath)}.log";
                                Utility.ExecuteScript(new ProcessRequest(
                                    buildToolScriptPath,
                                    buildToolLogPath,
                                    new string[]
                                    {
                                        Utility.ConvertPath(GradleEnvironment.BuildToolJarPath),
                                        helper.OutputPath + helper.OutputExt,
                                        helper.OutputPath + ".apks",
                                        PlayerSettings.Android.keystoreName,
                                        PlayerSettings.Android.keystorePass,
                                        PlayerSettings.Android.keyaliasName,
                                        PlayerSettings.Android.keyaliasPass
                                    },
                                    (boldToolResult) =>
                                    {
                                        if (boldToolResult.ExitCode != 0)
                                        {
                                            throw new Exception(
                                                $"{buildToolScriptPath} exit is {boldToolResult.ExitCode}.");
                                        }
                                    }
                                ));
                            }
                        }
                    ));

                    return ReturnCode.Success;
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    if (helper.IsBatchMode)
                    {
                        EditorApplication.Exit(1);
                    }

                    return ReturnCode.Error;
                }
            }

            private static class GradleEnvironment
            {
                private static string GradleScriptFileName =>
#if UNITY_EDITOR_WIN
                    "gradle-build.bat";
#else
                    "gradle-build.sh";
#endif
                public static string GradleScriptFilePath =>
                    Path.GetFullPath(
                        $"Packages/com.cyber1496.unitybuilder/Editor/Impl/Task/AndroidGradle/{GradleScriptFileName}");

                public static string BuildToolJarFileName =>
                    "bundletool-all-1.6.0.jar";

                public static string BuildToolJarPath =>
#if UNITY_EDITOR_WIN
                    Path.Combine(Path.GetDirectoryName(EditorApplication.applicationPath), $"Data/PlaybackEngines/AndroidPlayer/Tools/{BuildToolJarFileName}");
#else
                    Path.Combine(Path.GetDirectoryName(EditorApplication.applicationPath),
                        $"PlaybackEngines/AndroidPlayer/Tools/{BuildToolJarFileName}");
#endif
                private static string BuildToolScriptFileName =>
#if UNITY_EDITOR_WIN
                    "buildtool-build-apks.bat";
#else
                    "buildtool-build-apks.sh";
#endif
                public static string BuildToolScriptFilePath =>
                    Path.GetFullPath(
                        $"Packages/com.cyber1496.unitybuilder/Editor/Impl/Task/AndroidGradle/{BuildToolScriptFileName}");

                public static string GetBuildVariant(bool isDevelopment) =>
                    isDevelopment ? "debug" : "release";

                public static string GetOutputFilePath(string variant) =>
                    EditorUserBuildSettings.buildAppBundle
                        ? $"launcher/build/outputs/bundle/{variant}/launcher-{variant}"
                        : $"launcher/build/outputs/apk/{variant}/launcher-{variant}";
            }
        }
    }
}