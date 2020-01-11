using UnityEditor;
using UnityBuilder;
using UnityBuilder.StandardKit;
using UnityBuilder.ExternalToolKit;

public static class Example {
    static Example() {
        BuildProvider.RegisterBuildLogHandler(new MyLogHandler());

        BuildProvider.RegisterPreProcessor(new MyPreProcessor());
        BuildProvider.RegisterPostProcessor(new MyPostProcessor());

        // ExportProject => gradleによるapk出力
        BuildProvider.RegisterPostProcessor(new AndroidGradleProcessor());
        BuildProvider.RegisterPreProcessor(presettingProcessor);
    }
    //TODO:CLIからの変更
    static readonly PresettingProcessor presettingProcessor = new PresettingProcessor("example", "development");

#pragma warning disable IDE0051 // MenuItemはどこからも参照されない
    [MenuItem("Tools/Build/DoIt")] static void DoIt() => BuildProvider.Process();
    [MenuItem("Tools/Build/PreProcess")] static void PreProcess() => BuildProvider.PreProcess();
    [MenuItem("Tools/Build/PostProcess")] static void PostProcess() => BuildProvider.PostProcess();
    [MenuItem("Tools/ChangeConfig/example - product")] static void ChangeConfigExampleProduct() => presettingProcessor.Change("example", "product");
    [MenuItem("Tools/ChangeConfig/example - development")] static void ChangeConfigExampleDevelopment() => presettingProcessor.Change("example", "development");
#pragma warning restore IDE0051
}