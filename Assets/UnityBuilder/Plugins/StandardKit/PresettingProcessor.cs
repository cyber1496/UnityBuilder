using UnityEditor;
using Debug = UnityEngine.Debug;

namespace UnityBuilder.StandardKit {
    public partial class PresettingProcessor : IPreProcessor {
        public int PreOrder => 10;

        public PresettingProcessor(string newConfigName = "", string newSchemeName = "") {
            Change(newConfigName, newSchemeName);
        }
        public void Change(string newConfigName, string newSchemeName) {
            configName = newConfigName;
            schemeName = newSchemeName;
        }
        string configName;
        string schemeName;

        public void PreProcess(IBuildHelper helper) {
            ReadArguments(helper);
            Debug.Log($"PresettingProcessor.PreProcess:[{configName},{schemeName}]");
            if (Load(helper, configName, schemeName, out Scheme scheme)) {
                ApplyScheme(helper, scheme);
            }
        }
        void ReadArguments(IBuildHelper helper) {
            if (helper.BuildArguments.ContainsKey("-batchmode")) {
                if (helper.BuildArguments.ContainsKey("-config") && helper.BuildArguments.ContainsKey("-scheme")) {
                    configName = helper.BuildArguments["-config"];
                    schemeName = helper.BuildArguments["-scheme"];
                }
            }
        }
        void ApplyScheme(IBuildHelper helper, Scheme scheme) {
            Debug.Log(scheme.ToString());
            EditorUserBuildSettings.development = scheme.Development;
            EditorUserBuildSettings.allowDebugging = scheme.Development;
            PlayerSettings.SetApplicationIdentifier(helper.BuildTargetGroup, scheme.ApplicationIdentifier);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(helper.BuildTargetGroup, scheme.GetScriptingDefineSymbols(helper.BuildTargetGroup));
            PlayerSettings.companyName = scheme.CompanyName;
            PlayerSettings.productName = scheme.ProductName;
            PlayerSettings.iOS.appleDeveloperTeamID = scheme.IOS.AppleDeveloperTeamID;
            PlayerSettings.Android.useCustomKeystore = scheme.Android.UseCustomKeystore;
            PlayerSettings.Android.keystoreName = helper.GetReplacedPath(scheme.Android.KeystoreName);
            PlayerSettings.Android.keystorePass = scheme.Android.KeystorePass;
            PlayerSettings.Android.keyaliasName = scheme.Android.KeyaliasName;
            PlayerSettings.Android.keyaliasPass = scheme.Android.KeyaliasPass;
        }
    }
}