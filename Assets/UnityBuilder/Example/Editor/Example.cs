using UnityEditor;
using UnityBuilder;
using Debug = UnityEngine.Debug;

public static class Example {
    static Example() {
        BuildProvider.RegisterBuildLogHandler(new MyLogHandler());
        BuildProvider.RegisterProcessor(new MyProcessor());
        BuildProvider.RegisterPreProcessor(new MyPreProcessor());
        BuildProvider.RegisterPostProcessor(new MyPostProcessor());
    }
#pragma warning disable IDE0051 // MenuItemはどこからも参照されない
    [MenuItem("Tools/Build/DoIt")]
    static void DoIt() {
        // ビルド中はLoggerが切り替わるその確認
        Debug.Log("hoge");

        BuildProvider.Process();

        // ビルド中はLoggerが切り替わるその確認
        Debug.Log("fuga");
    }
#pragma warning restore IDE0051
}