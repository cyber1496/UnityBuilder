using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Injector;


namespace UnityBuilder
{
    [CreateAssetMenu(menuName = "UnityBuilder/BuildConfigurationApplyTaskAsset",
        fileName = "BuildConfigurationApplyTaskAsset")]
    public sealed class BuildConfigurationApplyTaskAsset : BuildTaskAsset
    {

        public override IBuildTask GetBuildTask(IBuildHelper helper)
            => new BuildConfigurationApplyTask(helper, this);

        private sealed class BuildConfigurationApplyTask : BuildTask
        {
            private readonly BuildConfigurationApplyTaskAsset taskAsset;

            public BuildConfigurationApplyTask(IBuildHelper helper, BuildConfigurationApplyTaskAsset asset)
                : base(helper, asset)
            {
                this.taskAsset = asset;
            }

            public override int Version => 1;

            protected override ReturnCode onRun()
            {
                var scheme = helper.Scheme;
                BuildLogger.Log(scheme.ToString());
                EditorUserBuildSettings.development = scheme.Development;
                EditorUserBuildSettings.allowDebugging = scheme.Development;
                PlayerSettings.SetApplicationIdentifier(helper.BuildTargetGroup, scheme.ApplicationIdentifier);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(helper.BuildTargetGroup,
                    scheme.GetScriptingDefineSymbols(helper.BuildTargetGroup));
                PlayerSettings.companyName = scheme.CompanyName;
                PlayerSettings.productName = scheme.ProductName;
                PlayerSettings.iOS.appleDeveloperTeamID = scheme.IOS.AppleDeveloperTeamID;
                EditorPrefs.SetString(IOS.PREFS_KEY_XCODE_PATH, helper.GetReplacedPath(scheme.IOS.XcodePath));
                EditorUserBuildSettings.buildAppBundle = scheme.Android.UseBuildAppBundle;
                EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
                //PlayerSettings.Android.useCustomKeystore = scheme.Android.UseCustomKeystore;
                PlayerSettings.Android.keystoreName = helper.GetReplacedPath(scheme.Android.KeystoreName);
                PlayerSettings.Android.keystorePass = scheme.Android.KeystorePass;
                PlayerSettings.Android.keyaliasName = scheme.Android.KeyaliasName;
                PlayerSettings.Android.keyaliasPass = scheme.Android.KeyaliasPass;
                EditorPrefs.SetString(Deploygate.PREFS_KEY, helper.GetReplacedPath(scheme.Deploygate.Authorization));
                return ReturnCode.Success;
            }
        }
    }
}