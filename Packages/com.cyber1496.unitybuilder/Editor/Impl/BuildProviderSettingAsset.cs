using UnityEngine;
using System;
using System.Linq;

namespace UnityBuilder
{
    [CreateAssetMenu(menuName = "UnityBuilder/BuildProviderSettingAsset", fileName = "BuildProviderSettingAsset")]
    public sealed class BuildProviderSettingAsset : ScriptableObject
    {
        [SerializeField] private UnityEngine.Object logHandlerAsset;
        [SerializeField] private UnityEngine.Object helperAsset;
        [SerializeField] private UnityEngine.Object[] taskAssets;

        private bool containsInterface(Type type, Type interfaceType)
            => type.GetInterfaces().Any(t => t == interfaceType);

        public void OnValidate()
        {
            logHandlerAsset = logHandlerAsset == null ||
                              containsInterface(logHandlerAsset.GetType(), typeof(IBuildLogHandlerAsset))
                ? logHandlerAsset
                : null;

            helperAsset = helperAsset == null || containsInterface(helperAsset.GetType(), typeof(IBuildHelperAsset))
                ? helperAsset
                : null;

            taskAssets = taskAssets
                .Where(asset => asset == null || containsInterface(asset.GetType(), typeof(IBuildTaskAsset)))
                .ToArray();
        }

        public IBuildLogHandler GetBuildLogHandler()
            => logHandlerAsset == null || containsInterface(logHandlerAsset.GetType(), typeof(IBuildLogHandlerAsset))
                ? ((IBuildLogHandlerAsset)(logHandlerAsset)).GetBuildLogHandler()
                : null;

        public IBuildHelper GetBuildHelper()
            => helperAsset == null || containsInterface(helperAsset.GetType(), typeof(IBuildHelperAsset))
                ? ((IBuildHelperAsset)(helperAsset)).GetBuildHelper()
                : null;

        public IBuildTaskAsset[] GetBuildTaskAsset()
            => taskAssets
                .Where(asset => asset == null || containsInterface(asset.GetType(), typeof(IBuildTaskAsset)))
                .Select(asset => (IBuildTaskAsset)asset)
                .OrderBy(asset => asset.Priority)
                .ToArray();
    }
}