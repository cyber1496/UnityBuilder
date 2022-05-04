using UnityEditor;
using UnityBuilder;

public static class Example {
    [MenuItem("Tools/Build/ForceReserializeAssets")]
    static void ForceReserializeAssets() {
        AssetDatabase.ForceReserializeAssets();
    }
    [MenuItem("Tools/Build/Run")]
    private static void Run() {
        var provider = new BuildProvider(AssetDatabase.LoadAssetAtPath<BuildProviderSettingAsset>("Assets/Example/Editor/BuildProviderSettingAsset.asset"));
        var returnCode = provider.Run();
        UnityEngine.Debug.Log(returnCode);
    }
    [MenuItem("Tools/Build/DryRun")]
    private static void DryRun() {
        var provider = new BuildProvider(AssetDatabase.LoadAssetAtPath<BuildProviderSettingAsset>("Assets/Example/Editor/BuildProviderSettingAsset.asset"));
        var returnCode = provider.DryRun();
        UnityEngine.Debug.Log(returnCode);
    }

}