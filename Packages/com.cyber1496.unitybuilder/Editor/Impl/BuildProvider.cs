using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Tasks;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Reporting;
namespace UnityBuilder {
    public sealed class BuildProvider {
        private readonly BuildProviderSettingAsset settings;
        public BuildProvider(BuildProviderSettingAsset settings) {
            this.settings = settings;
        }
        public ReturnCode DryRun() {

            var logHandler = settings.GetBuildLogHandler();
            logHandler.Apply();

            var helper = settings.GetBuildHelper();
            var contexts = new BuildContext();
            contexts.SetContextObject(new BundleBuildParameters(helper.BuildTarget, helper.BuildTargetGroup, Application.streamingAssetsPath));
            contexts.SetContextObject(new BuildInterfacesWrapper());
            contexts.SetContextObject(new BuildResults());
            contexts.SetContextObject(new BuildCallbacks());

            ReturnCode returnCode = BuildTasksRunner.Run(DefaultBuildTasks.Create(DefaultBuildTasks.Preset.PlayerScriptsOnly), contexts);

            logHandler.Revert();

            return returnCode;
        }
        public ReturnCode Run() {

            var logHandler = settings.GetBuildLogHandler();
            logHandler.Apply();

            var helper = settings.GetBuildHelper();
            var assets = settings.GetBuildTaskAsset();
            var contexts = new BuildContext();
            foreach (var asset in assets) {
                contexts.SetContextObject(asset);
            }

            ReturnCode returnCode = BuildTasksRunner.Run(
                assets.Select(asset => asset.GetBuildTask(helper)).ToArray(), contexts);

            logHandler.Revert();

            return returnCode;
        }
    }
}