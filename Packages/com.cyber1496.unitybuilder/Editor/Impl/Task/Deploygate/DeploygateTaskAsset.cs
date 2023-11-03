using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Injector;


namespace UnityBuilder {
    [CreateAssetMenu(menuName = "UnityBuilder/DeploygateTaskAsset", fileName = "DeploygateTaskAsset")]
    public sealed class DeploygateTaskAsset : BuildTaskAsset
    {
        public string ConfigureFilePath = "${HOME}/UnityBuilder.xml";
        
        public override IBuildTask GetBuildTask(IBuildHelper helper)
            => new DeploygateTask(helper, this);

        public sealed class DeploygateTask : BuildTask {
            private readonly DeploygateTaskAsset taskAsset;

            public DeploygateTask(IBuildHelper helper, DeploygateTaskAsset asset)
                : base(helper, asset)
            {
                this.taskAsset = asset;
            }

            public override int Version => 1;
            protected override ReturnCode onRun() {
                
                var authorization = Authorization.Load(helper.GetReplacedPath(taskAsset.ConfigureFilePath));
                if (!authorization.IsEnable) {
                    return ReturnCode.Success;
                }

                string scriptFileName
#if UNITY_EDITOR_WIN
                    = "deploygate.bat";
#else
                    = "deploygate.sh";
#endif
                string scriptPath = Path.GetFullPath($"Packages/com.cyber1496.unitybuilder/Editor/Impl/Task/Deploygate/{scriptFileName}");
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

                    return ReturnCode.Success;
                }
                catch (Exception ex) {
                    Debug.LogException(ex);
                    if (helper.IsBatchMode) {
                        EditorApplication.Exit(1);
                    }

                    return ReturnCode.Error;
                }
            }
            
			/// <example>
            /// <code>
            /// <?xml version="1.0" encoding="UTF-8"?>
            /// <Authorization>
            /// <Owner>---deploygate owner name---</Owner>
            /// <APIKey>---deploygate api key---</APIKey>
            /// </Authorization>
            /// </code>
			/// </example>
            public struct Authorization {
                public string Owner;
                public string APIKey;
                public bool IsEnable => !string.IsNullOrEmpty(Owner) && !string.IsNullOrEmpty(APIKey);
                public static Authorization Load(string filePath) {
                    if (!File.Exists(filePath)) {
                        return new Authorization();
                    }
                    var serializer = new XmlSerializer(typeof(Authorization));
                    var xmlSettings = new XmlReaderSettings() {
                        CheckCharacters = false,
                    };
                    using var streamReader = new StreamReader(filePath, Encoding.UTF8);
                    using var xmlReader = XmlReader.Create(streamReader, xmlSettings);
                    return (Authorization)serializer.Deserialize(xmlReader);
                }
            }
        }
    }
}