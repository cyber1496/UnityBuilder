using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace UnityBuilder.StandardKit {
    public partial class PresettingProcessor : IPreProcessor {
        public struct Configuration {
            [XmlAttribute("Identifier")] public string Identifier;
            public Scheme[] Schemes;

            public override string ToString() {
                string scehmes = "";
                foreach (var scehme in Schemes) {
                    scehmes += $"{scehme.ToString()},\n";
                }
                scehmes = scehmes.TrimEnd(',', '\n');
                return $"Identifier:{Identifier}, Schemes[\n{scehmes}\n]";
            }
        }
        public struct Scheme {
            [XmlAttribute("Identifier")] public string Identifier;
            public string ApplicationIdentifier;
            public string CompanyName;
            public string ProductName;
            public bool Development;
            public IOS IOS;
            public Android Android;
            public Deploygate Deploygate;
            public string GetScriptingDefineSymbols(BuildTargetGroup buildTargetGroup) {
                switch (buildTargetGroup) {
                    case BuildTargetGroup.iOS: return IOS.ScriptingDefineSymbols;
                    case BuildTargetGroup.Android: return Android.ScriptingDefineSymbols;
                    default: return "";
                }
            }

            public override string ToString() {
                return $"Identifier:{Identifier}, " +
                       $"ApplicationIdentifier:{ApplicationIdentifier}, " +
                       $"CompanyName:{CompanyName}, " +
                       $"ProductName:{ProductName}, " +
                       $"Development:{Development}, " +
                       $"IOS:[{IOS}], " +
                       $"Android:[{Android}]" +
                       $"Deploygate:[{Deploygate}]";
            }
        }
        public struct IOS {
            public string ScriptingDefineSymbols;
            public string AppleDeveloperTeamID;

            public override string ToString() {
                return $"ScriptingDefineSymbols:{ScriptingDefineSymbols}, " +
                       $"AppleDeveloperTeamID:{AppleDeveloperTeamID}";
            }
        }
        public struct Android {
            public string ScriptingDefineSymbols;
            public bool UseBuildAppBundle;
            public string KeystoreName;
            public string KeystorePass;
            public string KeyaliasName;
            public string KeyaliasPass;
            public bool UseCustomKeystore =>
                !string.IsNullOrEmpty(KeystoreName) &&
                !string.IsNullOrEmpty(KeystorePass) &&
                !string.IsNullOrEmpty(KeyaliasName) &&
                !string.IsNullOrEmpty(KeyaliasPass);

            public override string ToString() {
                return $"ScriptingDefineSymbols:{ScriptingDefineSymbols}, " +
                       $"UseBuildAppBundle:{UseBuildAppBundle}, " +
                       $"KeystoreName:{KeystoreName}, " +
                       $"KeystorePass:<private>, " +
                       $"KeyaliasName:{KeyaliasName}, " +
                       $"KeyaliasPass:<private>, " +
                       $"UseCustomKeystore:{UseCustomKeystore}";
            }
        }

        //todo:ExternalToolKitとの参照構成が良くない
        public struct Deploygate {
            public const string PREFS_KEY = "UnityBuilder.StandardKit.Deploygate.Authorization";
            public string Authorization;
            public override string ToString() {
                return $"Authorization:{Authorization}";
            }
        }

        bool Load(IBuildHelper helper, string configId, string schemeId, out Scheme scheme) {
            try {
                scheme = Load(helper)[configId].Schemes.ToDictionary(x => x.Identifier, x => x)[schemeId];
                return true;
            }
            catch (Exception e) {
                Debug.LogException(e);
                scheme = new Scheme();
                return true;
            }
        }
        Dictionary<string, Configuration> Load(IBuildHelper helper) {
            var collect = new Dictionary<string, Configuration>();
            var searchRoot = Path.GetFullPath(Path.Combine(helper.RootPath, "Configuration"));
            var filePaths = Directory.GetFiles(searchRoot, "*.xml", SearchOption.AllDirectories);
            var serializer = new XmlSerializer(typeof(Configuration));
            var xmlSettings = new XmlReaderSettings() {
                CheckCharacters = false,
            };
            foreach (var filePath in filePaths) {
                var result = Load(filePath, xmlSettings, serializer);
                collect.Add(result.Identifier, result);
            }
            return collect;
        }
        Configuration Load(string filePath, XmlReaderSettings xmlSettings, XmlSerializer serializer) {
            using (var streamReader = new StreamReader(filePath, Encoding.UTF8))
            using (var xmlReader = XmlReader.Create(streamReader, xmlSettings)) {
                return (Configuration)serializer.Deserialize(xmlReader);
            }
        }
    }
}