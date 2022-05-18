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
using Object = UnityEngine.Object;


namespace UnityBuilder
{
    [CreateAssetMenu(menuName = "UnityBuilder/AndroidGradleTaskAsset", fileName = "AndroidGradleTaskAsset")]
    public sealed class AndroidGradleTaskAsset : BuildTaskAsset
    {
        [SerializeField] private Object gradleBuiildScript;
        [SerializeField] private Object buildToolJar;
        [SerializeField] private Object buildToolScript;
        
        public override IBuildTask GetBuildTask(IBuildHelper helper)
            => new AndroidGradleTask(helper, this);

        private void Reset()
        {
            
        }

        private string getScriptFullPath(string shellPath)
        {
#if UNITY_EDITOR_WIN
            return Path.GetFullPath(shellPath).Replace(".sh", ".bat");
#else
            return Path.GetFullPath(shellPath);
#endif
        }

        private string getGradleBuildScriptPath()
        {
            if (gradleBuiildScript == null)
            {
                return getScriptFullPath("Packages/com.cyber1496.unitybuilder/Editor/Impl/Task/AndroidGradle/gradle-build.sh");
            }
                
            return getScriptFullPath(Utility.AssetObjectToPath(gradleBuiildScript));
        }
            
        private string getBuildToolJarPath()
        {
            if (buildToolJar == null)
            {
                return Path.Combine(Path.GetDirectoryName(EditorApplication.applicationPath), 
#if UNITY_EDITOR_WIN
                        "Data/PlaybackEngines/AndroidPlayer/Tools/bundletool-all-1.6.0.jar");
#else
                        "PlaybackEngines/AndroidPlayer/Tools/bundletool-all-1.6.0.jar");
#endif
            }
                
            return getScriptFullPath(Utility.AssetObjectToPath(buildToolJar));
        }

        private string getBuildToolScriptPath()
        {
            if (buildToolScript == null)
            {
                return getScriptFullPath("Packages/com.cyber1496.unitybuilder/Editor/Impl/Task/AndroidGradle/buildtool-build-apks.sh");
            }
                
            return getScriptFullPath(Utility.AssetObjectToPath(buildToolScript));
        }

        private string getBuildVariant(bool isDevelopment) =>
            isDevelopment ? "debug" : "release";

        private string getOutputFilePath(string variant) =>
            EditorUserBuildSettings.buildAppBundle
                ? $"launcher/build/outputs/bundle/{variant}/launcher-{variant}"
                : $"launcher/build/outputs/apk/{variant}/launcher-{variant}";
        
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

                string variant = taskAsset.getBuildVariant(EditorUserBuildSettings.development);
                string gradleScriptPath = taskAsset.getGradleBuildScriptPath();
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
                            Utility.ConvertPath(taskAsset.getOutputFilePath(variant)),
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
                                string buildToolScriptPath = taskAsset.getBuildToolScriptPath();
                                
                                string buildToolLogPath = $"Logs/{helper.BuildTarget}/{Path.GetFileName(buildToolScriptPath)}.log";

                                Utility.ExecuteScript(new ProcessRequest(
                                    buildToolScriptPath,
                                    buildToolLogPath,
                                    new string[]
                                    {
                                        Utility.ConvertPath(taskAsset.getBuildToolJarPath()),
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
            
        }
    }
}