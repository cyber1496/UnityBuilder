using System;
using UnityEngine;

namespace UnityBuilder {
    [CreateAssetMenu(menuName = "UnityBuilder/BuildLogHandlerAsset", fileName = "BuildLogHandlerAsset")]
    public class BuildLogHandlerAsset : ScriptableObject, IBuildLogHandlerAsset {
        public IBuildLogHandler GetBuildLogHandler() => new BuildLogHandler();
    }

    public class BuildLogHandler : IBuildLogHandler {
        public string Tag => "[BUILD]";
        private readonly ILogHandler defaultLogHandler;
        public BuildLogHandler() {
            defaultLogHandler = Debug.unityLogger.logHandler;
        }
        public void Apply() {
            Debug.unityLogger.logHandler = this;
        }
        public void Revert() {
            Debug.unityLogger.logHandler = defaultLogHandler;
        }
        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args) {
            defaultLogHandler.LogFormat(logType, context, $"{Tag}{format}", args);
        }
        public void LogException(Exception exception, UnityEngine.Object context) {
            defaultLogHandler.LogException(exception, context);
        }
    }
}