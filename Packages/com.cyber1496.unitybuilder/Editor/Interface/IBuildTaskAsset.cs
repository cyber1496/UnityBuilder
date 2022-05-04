using UnityEditor;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Injector;

namespace UnityBuilder {
    public interface IBuildTaskAsset : IContextObject {
        int Priority { get; }
        IBuildTask GetBuildTask(IBuildHelper helper);
        bool IsSupported(UnityEditor.BuildTarget target);
    }
}