using System.Collections.Generic;

namespace UnityBuilder {
    public interface IBuildArguments {
        string this[string index] { get; }
        bool ContainsKey(string key);
        void Add(string key, string value);
    }
}