using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;

namespace UnityBuilder {
    public abstract class BuildTask : IBuildTask {
        protected readonly IBuildHelper helper;
        protected readonly IBuildTaskAsset asset;
        public abstract int Version { get; }

        protected BuildTask(IBuildHelper helper, IBuildTaskAsset asset)
        {
            this.helper = helper;
            this.asset = asset;
        }
        
        public ReturnCode Run()
        {
            if (!asset.IsSupported(helper.BuildTarget))
            {
                return ReturnCode.Success;
            }

            return onRun();
        }

        protected abstract ReturnCode onRun();
    }
}