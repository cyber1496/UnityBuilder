using UnityEditor;
using System.IO;

namespace UnityBuilder.StandardKit {
    internal static class MenuItems {
#pragma warning disable IDE0051 // MenuItemはどこからも参照されない
        [MenuItem("Tools/UnityBuilder/ForceReserializeAssets")]
        static void ForceReserializeAssets() {
            AssetDatabase.ForceReserializeAssets();
        }
#pragma warning restore IDE0051
    }
}