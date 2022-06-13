using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEditor;


namespace UnityBuilder {

    public struct BuildConfiguration {
        [XmlAttribute("Identifier")] public string Identifier;
        public Scheme[] Schemes;

        public override string ToString() {
            string scehmes = "";
            foreach (var scehme in Schemes) {
                scehmes += $"{scehme},\n";
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
        public string GetScriptingDefineSymbols(BuildTargetGroup buildTargetGroup) => buildTargetGroup switch {
            BuildTargetGroup.iOS => IOS.ScriptingDefineSymbols,
            BuildTargetGroup.Android => Android.ScriptingDefineSymbols,
            _ => "",
        };

        public override string ToString() {
            return $"Identifier:{Identifier}, " +
                    $"ApplicationIdentifier:{ApplicationIdentifier}, " +
                    $"CompanyName:{CompanyName}, " +
                    $"ProductName:{ProductName}, " +
                    $"Development:{Development}, " +
                    $"IOS:[{IOS}], " +
                    $"Android:[{Android}]";
        }
    }

    public struct IOS {
        public string ScriptingDefineSymbols;
        public string AppleDeveloperTeamID;

        public string XcodePath;

        public const string PREFS_KEY_XCODE_PATH = "UnityBuilder.StandardKit.iOS.XcodePath";

        public override string ToString() {
            return $"ScriptingDefineSymbols:{ScriptingDefineSymbols}, " +
                    $"AppleDeveloperTeamID:{AppleDeveloperTeamID}," +
                    $"XcodePath:{XcodePath}";
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
}