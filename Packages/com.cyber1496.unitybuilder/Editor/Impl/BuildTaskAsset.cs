using System;
using UnityEngine;
using UnityEditor.Build.Pipeline.Interfaces;

namespace UnityBuilder {
    public abstract class BuildTaskAsset : ScriptableObject, IBuildTaskAsset {
        [SerializeField] private SupportBuildTarget supportBuildTargets;
        [SerializeField] private int priority;
        public int Priority => priority;
        public abstract IBuildTask GetBuildTask(IBuildHelper helper);

        public bool IsSupported(UnityEditor.BuildTarget target)
        {
            bool contains(SupportBuildTarget target)
                => (supportBuildTargets & target) == target;

            switch (target)
            {
                case UnityEditor.BuildTarget.StandaloneOSX: 
                    return contains(SupportBuildTarget.StandaloneOSX);
                case UnityEditor.BuildTarget.StandaloneWindows: 
                    return contains(SupportBuildTarget.StandaloneWindows);
                case UnityEditor.BuildTarget.StandaloneWindows64: 
                    return contains(SupportBuildTarget.StandaloneWindows64);
                case UnityEditor.BuildTarget.StandaloneLinux64: 
                    return contains(SupportBuildTarget.StandaloneLinux64);
                case UnityEditor.BuildTarget.iOS: 
                    return contains(SupportBuildTarget.iOS);
                case UnityEditor.BuildTarget.Android: 
                    return contains(SupportBuildTarget.Android);
                default:
                    throw new NotSupportedException($"{target} is Not Supported.");
            }
        }
    }
}