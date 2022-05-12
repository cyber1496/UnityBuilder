using System.Diagnostics;
using System;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEditor.Build.Pipeline.Utilities;


namespace UnityBuilder {
    public struct ProcessRequest {
        public string ScriptFilePath;
        public string LogFilePath;
        public string[] Args;
        public Action<ProcessResult> Callback;
        public ProcessRequest(string scriptFile, string logFilePath, string[] args, Action<ProcessResult> callback = null) {
            ScriptFilePath = scriptFile;
            LogFilePath = logFilePath;
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

    public class Utility {
        public static string AssetObjectToPath(UnityEngine.Object assetObject)
        {
            return AssetDatabase.GetAssetPath(assetObject);
        }
        public static string ConvertPath(string path) {
#if UNITY_EDITOR_WIN
            path = path.Replace("/", "\\");
#endif
            return path;
        }
        public static void ExecuteScript(ProcessRequest request) {
            EditorUtility.DisplayProgressBar("UnityBuilder.ExternalToolKit", $"ExecuteScript:{request.ScriptFilePath}", 0f);
            int exitCode = -1;
            try
            {
                var process = CreateProcess(request);
                BuildLogger.Log($"[{request.ProcessArguments}]");
                process.Start();
                string message = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                exitCode = process.ExitCode;
                process.Dispose();
                System.IO.File.WriteAllText(request.LogFilePath, message);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                request.Callback?.Invoke(new ProcessResult { ExitCode = exitCode });
            }
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
            p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            return p;
        }
    }
}