using System.Diagnostics;
using System;
using System.Text;
using System.Threading;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace UnityBuilder.ExternalToolKit {
    public struct ProcessRequest {
        public string ScriptFilePath;
        public string[] Args;
        public Action<ProcessResult> Callback;
        public ProcessRequest(string scriptFile, string[] args, Action<ProcessResult> callback = null) {
            ScriptFilePath = scriptFile;
            Args = args;
            Callback = callback;
        }
        public string ProcessArguments {
            get {
#if UNITY_EDITOR_WIN
                return $"/c \"{$"\"{ScriptFilePath}\" \"{string.Join("\" \"", Args)}\""}\"";
#else
                return $"-c \"sh {$"\"{ScriptFilePath}\" \"{string.Join("\" \"", Args)}\""}\"";
#endif
            }
        }
    }
    public struct ProcessResult {
        public int ExitCode;
    }

    public static class Utility {
#pragma warning disable IDE0051 // どこからも参照されない
        [InitializeOnLoadMethod]
        static void OnInitializeOnLoad() {
            context = SynchronizationContext.Current;
        }
#pragma warning restore IDE0051

        static SynchronizationContext context = null;
        public static string ConvertPath(string path) {
#if UNITY_EDITOR_WIN
            path = path.Replace("/", "\\");
#endif
            return path;
        }
        public static void ExecuteScript(ProcessRequest request) {
            context.Post((_) => EditorUtility.DisplayProgressBar("UnityBuilder.ExternalToolKit", $"ExecuteScript:{request.ScriptFilePath}", 0f), null);
            var process = CreateProcess(request);
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            process.CancelOutputRead();
            process.CancelErrorRead();
            int exitCode = process.ExitCode;
            process.Close();
            context.Post((_) => EditorUtility.ClearProgressBar(), null);
            context.Post((_) => request.Callback?.Invoke(new ProcessResult { ExitCode = exitCode }), null);
        }
        static string ProcessFileName {
            get {
#if UNITY_EDITOR_WIN
                return Environment.GetEnvironmentVariable("ComSpec");
#else
                return "/bin/bash";
#endif
            }
        }
        static Process CreateProcess(ProcessRequest request) {
            var p = new Process();
            p.StartInfo.FileName = ProcessFileName;
            p.StartInfo.Arguments = request.ProcessArguments;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.OutputDataReceived += (sender, e) => {
                context.Post((_) => {
                    if (!string.IsNullOrEmpty(e.Data)) {
                        Debug.Log(e.Data);
                    }
                }, null);
            };
            p.ErrorDataReceived += (sender, e) => {
                context.Post((_) => {
                    if (!string.IsNullOrEmpty(e.Data)) {
                        Debug.LogError(e.Data);
                    }
                }, null);
            };
            p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            return p;
        }
    }
}