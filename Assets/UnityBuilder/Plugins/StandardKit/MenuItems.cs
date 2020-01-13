using UnityEditor;
using System.IO;

namespace UnityBuilder.StandardKit {
    internal static class MenuItems {
        static string PackageFile { get; } = "Packages/UnityBuilder.unitypackage";
        static string AssetsRoot { get; } = "Assets/UnityBuilder";

#pragma warning disable IDE0051 // MenuItemはどこからも参照されない
        [MenuItem("Tools/UnityBuilder/Export Package")]
        static void ExportPackage() {
            AssetDatabase.ExportPackage(AssetsRoot, PackageFile,
                ExportPackageOptions.Interactive |
                ExportPackageOptions.Recurse |
                ExportPackageOptions.Default);
        }
        [MenuItem("Tools/UnityBuilder/Import Package")]
        static void ImportPackage() {
            if (File.Exists(PackageFile)) {
                AssetDatabase.ImportPackage(PackageFile, true);
            }
        }
#pragma warning restore IDE0051
    }
}