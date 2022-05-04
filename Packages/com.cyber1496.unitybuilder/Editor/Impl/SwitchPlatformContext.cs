using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;

namespace UnityBuilder {
    public interface ISwitchPlatformContext : IContextObject {
        BuildTargetGroup Group { get; }
        BuildTarget Target { get; }
    }
    public class SwitchPlatformContext : ISwitchPlatformContext {
        public BuildTargetGroup Group { get; }
        public BuildTarget Target { get; }

        public SwitchPlatformContext(BuildTargetGroup group, BuildTarget target) {
            Group = group;
            Target = target;
        }
    }
}