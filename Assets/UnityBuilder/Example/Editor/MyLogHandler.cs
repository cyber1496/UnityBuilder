using System;
using System.IO;
using UnityEngine;
using UnityBuilder;
using Debug = UnityEngine.Debug;

public class MyLogHandler : IBuildLogHandler {
    public string Tag => "[MYBUILD]";
    readonly ILogHandler defaultLogHandler = Debug.unityLogger.logHandler;
    private FileStream fileStream;
    private StreamWriter streamWriter;
    public void PreProcess(IBuildHelper helper) {
        string filePath = Application.dataPath.Replace("Assets", "Logs/") + helper.OutputFile + ".log";
        fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        streamWriter = new StreamWriter(fileStream);
        Debug.unityLogger.logHandler = this;
    }
    public void PostProcess(IBuildHelper helper) {
        Debug.unityLogger.logHandler = defaultLogHandler;
        streamWriter.Flush();
        streamWriter.Dispose();
        streamWriter = null;
        fileStream.Dispose();
        fileStream = null;
    }
    void WriteLog(string msg) {
        streamWriter.WriteLine(msg);
        streamWriter.Flush();
    }
    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args) {
        WriteLog(string.Format(format, args));
        defaultLogHandler.LogFormat(logType, context, $"{Tag}{format}", args);
    }
    public void LogException(Exception exception, UnityEngine.Object context) {
        WriteLog(exception.ToString());
        defaultLogHandler.LogException(exception, context);
    }
}