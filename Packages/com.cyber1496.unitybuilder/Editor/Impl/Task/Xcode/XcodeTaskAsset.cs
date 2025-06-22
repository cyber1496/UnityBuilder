using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Injector;
using System.Diagnostics;

#if UNITY_EDITOR_OSX
using UnityEditor.iOS.Xcode;
#endif

namespace UnityBuilder {
    [CreateAssetMenu(menuName = "UnityBuilder/XcodeTaskAsset", fileName = "XcodeTaskAsset")]
    public sealed class XcodeTaskAsset : BuildTaskAsset {
        
        public override IBuildTask GetBuildTask(IBuildHelper helper)
            => new XcodeTask(helper, this);

        public sealed class XcodeTask : BuildTask {
            private readonly XcodeTaskAsset taskAsset;
            public XcodeTask(IBuildHelper helper, XcodeTaskAsset asset)
                : base(helper, asset)
            {
                this.taskAsset = asset;
            }
            public override int Version => 1;
            
            protected override ReturnCode onRun() {
                try {
                    // デバッグ情報の出力
                    UnityEngine.Debug.Log("=== XcodeTask Environment Debug ===");
                    UnityEngine.Debug.Log($"Current Environment Type: {helper.GetEnvironmentType()}");
                    UnityEngine.Debug.Log($"Is Mac Environment: {helper.IsEnvironment(EnvironmentType.Mac)}");
                    UnityEngine.Debug.Log($"Is WSL Environment: {helper.IsEnvironment(EnvironmentType.WSL)}");
                    UnityEngine.Debug.Log($"Is Windows_NT Environment: {helper.IsEnvironment(EnvironmentType.Windows_NT)}");
                    
                    // 環境変数の確認
                    string envVar = Environment.GetEnvironmentVariable("ENVIRONMENT");
                    UnityEngine.Debug.Log($"ENVIRONMENT Variable: {envVar ?? "null"}");
                    
                    // iOSビルドはMac環境でのみサポート（WSLはスクリプト側で判定）
                    if (!helper.IsEnvironment(EnvironmentType.Mac)) {
                        // Windows環境の場合、WSLが利用可能かチェック
                        if (helper.IsEnvironment(EnvironmentType.Windows_NT)) {
                            UnityEngine.Debug.Log("Windows environment detected. Attempting WSL build...");
                            ExecuteWSLBuild(helper);
                            return ReturnCode.Success;
                        }
                        
                        UnityEngine.Debug.LogWarning("iOS build is only supported on Mac or WSL environment.");
                        UnityEngine.Debug.LogWarning($"Current environment: {helper.GetEnvironmentType()}");
                        return ReturnCode.Success;
                    }

                    // Mac環境でのビルド実行
                    UnityEngine.Debug.Log("Executing Mac build path...");
                    ExecuteMacBuild(helper);

                    return ReturnCode.Success;
                }
                catch (Exception ex) {
                    UnityEngine.Debug.LogException(ex);
                    if (helper.IsBatchMode) {
                        EditorApplication.Exit(1);
                    }
                    return ReturnCode.Error;
                }
            }

            private void ExecuteMacBuild(IBuildHelper helper) {
#if UNITY_EDITOR_OSX
                UnityEngine.Debug.Log("Executing iOS build on Mac environment using xcode-build.sh.");
                
                // PBXProjectの設定
                string pbxPath = PBXProject.GetPBXProjectPath(helper.OutputPath);
                PBXProject pbx = new PBXProject();
                pbx.ReadFromString(File.ReadAllText(pbxPath));
                
                // Bitcodeを無効化
                string target = pbx.GetUnityMainTargetGuid();
                pbx.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
                target = pbx.GetUnityFrameworkTargetGuid();
                pbx.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
                File.WriteAllText(pbxPath, pbx.WriteToString());
                
                // ネイティブmacOS環境でのビルド
                string xcodePath = EditorPrefs.GetString("UnityBuilder.StandardKit.iOS.XcodePath");
                string scriptPath = Path.GetFullPath("Packages/com.cyber1496.unitybuilder/Editor/Impl/Task/Xcode/xcode-build.sh");
                if (!File.Exists(scriptPath)) {
                    throw new Exception($"xcode-build.sh not found at {scriptPath}. Please create the Mac build script.");
                }

                // ExportOptions.plistの作成
                string exportOptionPlistPath = CreateExportOption(helper.OutputPath);
                string logPath = $"Logs/{helper.BuildTarget}/xcode-build.log";
                string outputPath = helper.OutputPath;
                string configuration = EditorUserBuildSettings.development ? "Debug" : "Release";

                UnityEngine.Debug.Log($"Executing script: {scriptPath}");
                UnityEngine.Debug.Log($"Output path: {outputPath}");
                UnityEngine.Debug.Log($"Configuration: {configuration}");

                Utility.ExecuteScript(new ProcessRequest(
                    scriptPath,
                    logPath,
                    new string[] {
                        outputPath,
                        xcodePath,
                        configuration,
                        exportOptionPlistPath
                    },
                    (result) => {
                        if (result.ExitCode != 0) {
                            throw new Exception($"xcode-build.sh failed with exit code {result.ExitCode}.");
                        }
                        UnityEngine.Debug.Log("Mac iOS build completed successfully.");
                    }
                ));
#else
                throw new Exception("Mac build is only available on macOS with UNITY_EDITOR_OSX defined.");
#endif
            }

            private void ExecuteWSLBuild(IBuildHelper helper) {
                UnityEngine.Debug.Log("Executing iOS build on WSL environment using xtool-build.sh.");
                
                // WSL環境での専用スクリプトを使用したビルド
                string scriptPath = Path.GetFullPath("Packages/com.cyber1496.unitybuilder/Editor/Impl/Task/Xcode/xtool-build.sh");
                if (!File.Exists(scriptPath)) {
                    throw new Exception($"xtool-build.sh not found at {scriptPath}. Please create the WSL build script.");
                }

                // WindowsパスをWSLパスに変換（エスケープなし）
                string wslScriptPath = ConvertToWSLPathNoEscape(scriptPath);
                string wslOutputPath = ConvertToWSLPathNoEscape(helper.OutputPath);
                string logPath = $"Logs/{helper.BuildTarget}/xtool-build.log";
                string configuration = EditorUserBuildSettings.development ? "Debug" : "Release";

                UnityEngine.Debug.Log($"WSL script path: {wslScriptPath}");
                UnityEngine.Debug.Log($"WSL output path: {wslOutputPath}");
                UnityEngine.Debug.Log($"Configuration: {configuration}");

                // WSLコマンドでスクリプトを実行（パスを引用符で囲む）
                string wslCommand = $"wsl bash '{wslScriptPath}' '{wslOutputPath}' '{configuration}'";
                
                UnityEngine.Debug.Log($"Executing WSL command: {wslCommand}");

                Utility.ExecuteScript(new ProcessRequest(
                    "cmd.exe",
                    logPath,
                    new string[] {
                        "/c",
                        wslCommand
                    },
                    (result) => {
                        if (result.ExitCode != 0) {
                            throw new Exception($"WSL xtool-build.sh failed with exit code {result.ExitCode}.");
                        }
                        UnityEngine.Debug.Log("WSL iOS build completed successfully.");
                    }
                ));
            }

#if UNITY_EDITOR_OSX
            private string CreateExportOption(string outputPath) {
                string path = $"{outputPath}/ExportOptions.plist";
                var plist = new PlistDocument();
                plist.root.SetString("method", "development");
                plist.root.SetString("teamID", PlayerSettings.iOS.appleDeveloperTeamID);
                plist.WriteToFile(path);
                return path;
            }
#endif

            private string ConvertToWSLPathNoEscape(string windowsPath) {
                // WindowsパスをWSLパスに変換（エスケープなし）
                string wslPath = windowsPath.Replace("\\", "/");
                
                // ドライブ文字の変換（例: F: -> /mnt/f）
                if (wslPath.Length >= 2 && wslPath[1] == ':') {
                    char driveLetter = char.ToLower(wslPath[0]);
                    wslPath = $"/mnt/{driveLetter}{wslPath.Substring(2)}";
                }
                
                UnityEngine.Debug.Log($"Converted path (no escape): {windowsPath} -> {wslPath}");
                return wslPath;
            }
        }
    }
}