using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEditor;

namespace UnityBuilder {
    [CreateAssetMenu(menuName = "UnityBuilder/BuildHelperAsset", fileName = "BuildHelperAsset")]
    public sealed class BuildHelperAsset : ScriptableObject, IBuildHelperAsset {

        [SerializeField] private string defaultConfigName;
        [SerializeField] private string defaultSchemeName;
        public IBuildHelper GetBuildHelper() => new BuildHelper(defaultConfigName, defaultSchemeName);
    }

    public sealed class BuildHelper : IBuildHelper {
        public string RootPath => Application.dataPath.Replace("Assets", "");
        public string[] TargetScenes => (from scene in EditorBuildSettings.scenes select scene.path).ToArray();
        public string OutputPath {
            get {
                switch (BuildTarget) {
                    case BuildTarget.Android:
                        bool exportExternalProject = EditorUserBuildSettings.exportAsGoogleAndroidProject;
                        return $"build/{BuildTarget}/{OutputFile}" + (exportExternalProject ? "" : $"{OutputExt}");
                    case BuildTarget.StandaloneWindows:
                    case BuildTarget.StandaloneWindows64:
                        return $"build/{BuildTarget}/{OutputFile}{OutputExt}";
                    case BuildTarget.StandaloneOSX:
                    case BuildTarget.iOS:
                        return $"build/{BuildTarget}/{OutputFile}";
                    default: return "";
                }
            }
        }
        public string OutputFile => PlayerSettings.productName;
        public string OutputExt {
            get {
                switch (BuildTarget) {
                    case BuildTarget.Android: {
                            return EditorUserBuildSettings.buildAppBundle ? ".aab" : ".apk";
                        }
                    case BuildTarget.iOS: return ".ipa";
                    case BuildTarget.StandaloneWindows:
                    case BuildTarget.StandaloneWindows64: return ".exe";
                    case BuildTarget.StandaloneOSX: return ".app";
                    default: return "";
                }
            }
        }
        public BuildTarget BuildTarget => EditorUserBuildSettings.activeBuildTarget;
        public BuildTargetGroup BuildTargetGroup => EditorUserBuildSettings.selectedBuildTargetGroup;
        public BuildOptions BuildOptions { get => BuildOptions.None; }
        public IBuildArguments BuildArguments { get; } = new BuildArguments();
        public bool IsBatchMode => BuildArguments.ContainsKey("-batchmode");
        public Scheme Scheme { get; private set; }

        public BuildHelper(string configName, string schemeName) {
            Scheme = Load(configName, schemeName);
        }

        public string GetReplacedPath(string path) {
            path = path.Replace("${PROJECT_ROOT}/", RootPath);
            path = path.Replace("${HOME}", Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            path = Path.GetFullPath(path);
            return path;
        }

        private Scheme Load(string configName, string schemeName) {
            if (BuildArguments.ContainsKey("-batchmode")) {
                if (BuildArguments.ContainsKey("-config") && BuildArguments.ContainsKey("-scheme")) {
                    configName = BuildArguments["-config"];
                    schemeName = BuildArguments["-scheme"];
                }
            }
            return LoadScheme(configName, schemeName);
        }

        private Scheme LoadScheme(string configName, string schemeName) {
            try {
                return LoadConfiguration()[configName].Schemes.ToDictionary(x => x.Identifier, x => x)[schemeName];
            }
            catch (Exception e) {
                Debug.LogException(e);
                return new Scheme();
            }
        }

        private Dictionary<string, BuildConfiguration> LoadConfiguration() {
            var collect = new Dictionary<string, BuildConfiguration>();
            var searchRoot = Path.GetFullPath(Path.Combine(RootPath, "Configuration"));
            var filePaths = Directory.GetFiles(searchRoot, "*.xml", SearchOption.AllDirectories);
            var serializer = new XmlSerializer(typeof(BuildConfiguration));
            var xmlSettings = new XmlReaderSettings() {
                CheckCharacters = false,
            };
            foreach (var filePath in filePaths) {
                var result = LoadConfiguration(filePath, xmlSettings, serializer);
                collect.Add(result.Identifier, result);
            }
            return collect;
        }

        private BuildConfiguration LoadConfiguration(string filePath, XmlReaderSettings xmlSettings, XmlSerializer serializer) {
            using var streamReader = new StreamReader(filePath, Encoding.UTF8);
            using var xmlReader = XmlReader.Create(streamReader, xmlSettings);
            return (BuildConfiguration)serializer.Deserialize(xmlReader);
        }
    }
}