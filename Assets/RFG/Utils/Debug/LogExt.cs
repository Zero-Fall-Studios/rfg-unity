using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

#if !(DEVELOPMENT_BUILD || UNITY_EDITOR)
// Only show exceptions
// Debug.unityLogger.filterLogType = LogType.Exception;
// Debug.unityLogger.logEnabled = false; 
#endif

namespace RFG
{
  public class LoggerConfig
  {
    public string className;
    public bool logClassName;
    public bool logGameObjectName;
    public bool logThreadId;
    public bool logGameTime;

    public LoggerConfig(string className, bool logClassName = true, bool logGameObjectName = true, bool logThreadId = true, bool logGameTime = true)
    {
      this.className = className;
      this.logClassName = logClassName;
      this.logGameObjectName = logGameObjectName;
      this.logThreadId = logThreadId;
      this.logGameTime = logGameTime;
    }
  }

  public static class LogExt
  {

    public static readonly Dictionary<Type, (Logger, LoggerConfig)> loggers = new Dictionary<Type, (Logger, LoggerConfig)>();

    private static void LogFormat(UnityEngine.Object unityObj, LogType logType, string format, params object[] args)
    {
#if (DEVELOPMENT_BUILD || UNITY_EDITOR)
      (Logger logger, LoggerConfig lc) = GetLoggerByType(unityObj.GetType());
      logger.LogFormat(logType, unityObj, string.Format("{0}{1}{2}{3}{4}",
          lc.logThreadId ? "[" + Thread.CurrentThread.ManagedThreadId.ToString() + "] " : "",
          lc.logGameTime ? "[" + Time.time + "]" : "",
          lc.logClassName ? "(" + lc.className + ")" : "",
          lc.logGameObjectName ? "(" + unityObj.name + ") " : "",
          format),
          args);
#endif
    }
    public static void Log(this UnityEngine.Object unityObj, string format, params object[] args)
    {
      LogFormat(unityObj, LogType.Log, format, args);
    }
    public static void Warn(this UnityEngine.Object unityObj, string format, params object[] args)
    {
      LogFormat(unityObj, LogType.Warning, format, args);
    }

    public static void Error(this UnityEngine.Object unityObj, string format, params object[] args)
    {
      LogFormat(unityObj, LogType.Error, format, args);
    }

    private static void AddType(Type type)
    {
      if (!loggers.ContainsKey(type))
      {
        LoggerConfig lc = new LoggerConfig(string.Format("{0}", type.Name));
        loggers.Add(type, (new Logger(Debug.unityLogger.logHandler), lc));
      }
    }

    public static (Logger logger, LoggerConfig loggerConfig) GetLoggerByType(Type type)
    {
      AddType(type);
      return loggers[type];
    }

    public static (Logger logger, LoggerConfig loggerConfig) GetLoggerByType<T>()
    {
      return GetLoggerByType(typeof(T));
    }

    private static void LogStatic<T>(LogType logType, string format, params object[] args)
    {
#if (DEVELOPMENT_BUILD || UNITY_EDITOR)
      (Logger logger, LoggerConfig lc) = GetLoggerByType<T>();
      logger.LogFormat(logType, string.Format("{0}{1}{2}{3}{4}",
          lc.logThreadId ? "[" + Thread.CurrentThread.ManagedThreadId.ToString() + "] " : "",
          lc.logGameTime ? "[" + string.Format("{0:0.00}", Time.time) + "]" : "",
          lc.logClassName ? "(" + lc.className + ")" : "",
          lc.logGameObjectName ? "(<static>) " : "",
          format),
          args);
# endif
    }

    public static void Log<T>(string format, params object[] args)
    {
      LogStatic<T>(LogType.Log, format, args);
    }

    public static void Warn<T>(string format, params object[] args)
    {
      LogStatic<T>(LogType.Warning, format, args);
    }

    public static void Error<T>(string format, params object[] args)
    {
      LogStatic<T>(LogType.Error, format, args);
    }
  }
}