using System;
using UnityEngine;

namespace UnityBuilder.StandardKit {
    public class BuildLogHandler : IBuildLogHandler {
        public string Tag => "[BUILD]";
        readonly ILogHandler defaultLogHandler = Debug.unityLogger.logHandler;
        public void PreProcess(IBuildHelper helper) {
            Debug.unityLogger.logHandler = this;
        }
        public void PostProcess(IBuildHelper helper) {
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