using UnityEditor;
using UnityBuilder;
using UnityBuilder.ExternalToolKit;

public static class Example {
    static Example() {
        BuildProvider.RegisterBuildLogHandler(new MyLogHandler());

        BuildProvider.RegisterPreProcessor(new MyPreProcessor());
        BuildProvider.RegisterPostProcessor(new MyPostProcessor());

        // ExportProject => gradleによるapk出力
        BuildProvider.RegisterPostProcessor(new AndroidGradleProcessor());
    }
#pragma warning disable IDE0051 // MenuItemはどこからも参照されない
    [MenuItem("Tools/Build/DoIt")] static void DoIt() => BuildProvider.Process();
#pragma warning restore IDE0051
}