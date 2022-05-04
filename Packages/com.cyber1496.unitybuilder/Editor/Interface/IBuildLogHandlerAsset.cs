using UnityEngine;

namespace UnityBuilder {

    public interface IBuildLogHandlerAsset {
        IBuildLogHandler GetBuildLogHandler();
    }

    public interface IBuildLogHandler : ILogHandler {
        string Tag { get; }
        void Apply();
        void Revert();
    }
}