using System;
using System.Collections.Generic;

namespace UnityBuilder.StandardKit {
    public class BuildArguments : IBuildArguments {
        Dictionary<string, string> parameters { get; } = new Dictionary<string, string>();
        public BuildArguments() {
            ReadCommandLineArgs();
        }
        void ReadCommandLineArgs() {
            parameters.Clear();
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++) {
                if (args[i].IndexOf('-') == 0) {
                    string key = args[i];
                    string value = "";
                    if (args.Length > i + 1 && args[i + 1].IndexOf('-') != 0) {
                        value = args[i + 1];
                        i++;
                    }
                    if (!string.IsNullOrEmpty(key)) {
                        Add(key, value);
                    }
                }
            }
        }
        public string this[string key] => parameters[key];
        public bool ContainsKey(string key) => parameters.ContainsKey(key);
        public void Add(string key, string value) {
            if (!parameters.ContainsKey(key)) {
                parameters.Add(key, value);
            }
            else {
                parameters[key] = value;
            }
        }
    }
}