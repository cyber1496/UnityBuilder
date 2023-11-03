using UnityBuilder;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEngine;

public static class Example {
    [MenuItem("Tools/Build/ForceReserializeAssets")]
    private static void ForceReserializeAssets() {
        AssetDatabase.ForceReserializeAssets();
    }

    [MenuItem("Tools/Build/Run")]
    private static void Run() {
        BuildProvider provider =
            new BuildProvider(
                AssetDatabase.LoadAssetAtPath<BuildProviderSettingAsset>(
                    "Assets/Example/Editor/BuildProviderSettingAsset.asset"));
        ReturnCode returnCode = provider.Run();
        Debug.Log(returnCode);
    }

    [MenuItem("Tools/Build/DryRun")]
    private static void DryRun() {
        BuildProvider provider =
            new BuildProvider(
                AssetDatabase.LoadAssetAtPath<BuildProviderSettingAsset>(
                    "Assets/Example/Editor/BuildProviderSettingAsset.asset"));
        ReturnCode returnCode = provider.DryRun();
        Debug.Log(returnCode);
    }
}