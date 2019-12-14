using UnityEngine;

namespace UnityBuilder {
    public interface IBuildLogHandler : ILogHandler {
        string Tag { get; }
        void PreProcess();
        void PostProcess();
    }
}