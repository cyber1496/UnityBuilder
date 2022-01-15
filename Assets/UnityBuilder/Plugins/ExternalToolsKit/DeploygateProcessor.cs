using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace UnityBuilder.ExternalToolKit {
    public sealed class DeploygateProcessor : IPostProcessor {
        public int PostOrder => 20;
        public void PostProcess(IBuildHelper helper) {
            var authorization = Authorization.Load(EditorPrefs.GetString("UnityBuilder.StandardKit.Deploygate.Authorization"));
            Debug.Log($"DeploygateProcessor.PostProcess[{authorization.Owner},{authorization.APIKey}]");
            if (!authorization.IsEnable) {
                return;
            }
            string scriptPath = Path.Combine(helper.RootPath, XcodeEnvironment.ScriptFilePath);
            string logPath = $"Logs/{helper.BuildTarget}/{Path.GetFileName(scriptPath)}.log";
            string outputPath = helper.OutputPath;
            try {
                Utility.ExecuteScript(new ProcessRequest(
                    scriptPath,
                    logPath,
                    new string[] {
                        authorization.Owner,
                        authorization.APIKey,
                        outputPath + helper.OutputExt,
                        "True"
                    },
                    (result) => {
                        if (result.ExitCode != 0) {
                            throw new Exception($"{scriptPath} exit is {result.ExitCode}.");
                        }
                    }
                ));
            }
            catch (Exception ex) {
                Debug.LogException(ex);
                if (helper.IsBatchMode) {
                    EditorApplication.Exit(1);
                }
            }
        }
        public struct Authorization {
            public string Owner;
            public string APIKey;
            public bool IsEnable => !string.IsNullOrEmpty(Owner) && !string.IsNullOrEmpty(APIKey);
            public static Authorization Load(string filePath) {
                Debug.Log(filePath);
                if (!File.Exists(filePath)) {
                    return new Authorization();
                }
                var serializer = new XmlSerializer(typeof(Authorization));
                var xmlSettings = new XmlReaderSettings() {
                    CheckCharacters = false,
                };
                using (var streamReader = new StreamReader(filePath, Encoding.UTF8))
                using (var xmlReader = XmlReader.Create(streamReader, xmlSettings)) {
                    return (Authorization)serializer.Deserialize(xmlReader);
                }
            }
        }
        static class XcodeEnvironment {
            static string ScriptFileName
#if UNITY_EDITOR_WIN
                => "deploygate.bat";
#else
                => "deploygate.sh";
#endif
            public static string ScriptFilePath
                => $"Assets/UnityBuilder/Plugins/ExternalToolsKit/DeploygateProcessor/{ScriptFileName}";
        }
    }
}