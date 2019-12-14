using System;
using UnityEngine;

namespace UnityBuilder {
    public class StandardBuildLogHandler : IBuildLogHandler {
        public string Tag => "[BUILD]";
        readonly ILogHandler defaultLogHandler = Debug.unityLogger.logHandler;
        public void PreProcess() {
            Debug.unityLogger.logHandler = this;
        }
        public void PostProcess() {
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