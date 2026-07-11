using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public static class Debug
{
	private static string TimeStamp()
	{
		return DateTime.UtcNow.ToString("[HH:mm:ss.fff] [") + Thread.CurrentThread.ManagedThreadId + "] ";
	}

	private static void WriteTimeStamped(params object[] objs)
	{
		Console.WriteLine(global::Debug.TimeStamp() + DebugUtil.BuildString(objs));
	}

	public static bool isDebugBuild
	{
		get
		{
			return global::UnityEngine.Debug.isDebugBuild;
		}
	}

	public static bool developerConsoleVisible
	{
		get
		{
			return global::UnityEngine.Debug.developerConsoleVisible;
		}
		set
		{
			global::UnityEngine.Debug.developerConsoleVisible = value;
		}
	}

	public static void Break()
	{
	}

	public static void LogException(Exception exception)
	{
		if (global::Debug.s_loggingDisabled)
		{
			return;
		}
		global::UnityEngine.Debug.LogException(exception);
	}

	public static void Log(object obj)
	{
		if (global::Debug.s_loggingDisabled)
		{
			return;
		}
		global::Debug.WriteTimeStamped(new object[] { "[INFO]", obj });
	}

	public static void Log(object obj, global::UnityEngine.Object context)
	{
		if (global::Debug.s_loggingDisabled)
		{
			return;
		}
		global::Debug.WriteTimeStamped(new object[]
		{
			"[INFO]",
			(context != null) ? context.name : "null",
			obj
		});
	}

	public static void LogFormat(string format, params object[] args)
	{
		if (global::Debug.s_loggingDisabled)
		{
			return;
		}
		global::Debug.WriteTimeStamped(new object[]
		{
			"[INFO]",
			string.Format(format, args)
		});
	}

	public static void LogFormat(global::UnityEngine.Object context, string format, params object[] args)
	{
		if (global::Debug.s_loggingDisabled)
		{
			return;
		}
		global::Debug.WriteTimeStamped(new object[]
		{
			"[INFO]",
			(context != null) ? context.name : "null",
			string.Format(format, args)
		});
	}

	public static void LogWarning(object obj)
	{
		if (global::Debug.s_loggingDisabled)
		{
			return;
		}
		global::Debug.WriteTimeStamped(new object[] { "[WARNING]", obj });
	}

	public static void LogWarning(object obj, global::UnityEngine.Object context)
	{
		if (global::Debug.s_loggingDisabled)
		{
			return;
		}
		global::Debug.WriteTimeStamped(new object[]
		{
			"[WARNING]",
			(context != null) ? context.name : "null",
			obj
		});
	}

	public static void LogWarningFormat(string format, params object[] args)
	{
		if (global::Debug.s_loggingDisabled)
		{
			return;
		}
		global::Debug.WriteTimeStamped(new object[]
		{
			"[WARNING]",
			string.Format(format, args)
		});
	}

	public static void LogWarningFormat(global::UnityEngine.Object context, string format, params object[] args)
	{
		if (global::Debug.s_loggingDisabled)
		{
			return;
		}
		global::Debug.WriteTimeStamped(new object[]
		{
			"[WARNING]",
			(context != null) ? context.name : "null",
			string.Format(format, args)
		});
	}

	public static void LogError(object obj)
	{
		if (global::Debug.s_loggingDisabled)
		{
			return;
		}
		global::Debug.WriteTimeStamped(new object[] { "[ERROR]", obj });
		global::UnityEngine.Debug.LogError(obj);
	}

	public static void LogError(object obj, global::UnityEngine.Object context)
	{
		if (global::Debug.s_loggingDisabled)
		{
			return;
		}
		global::Debug.WriteTimeStamped(new object[]
		{
			"[ERROR]",
			(context != null) ? context.name : "null",
			obj
		});
		global::UnityEngine.Debug.LogError(obj, context);
	}

	public static void LogErrorFormat(string format, params object[] args)
	{
		if (global::Debug.s_loggingDisabled)
		{
			return;
		}
		global::Debug.WriteTimeStamped(new object[]
		{
			"[ERROR]",
			string.Format(format, args)
		});
		global::UnityEngine.Debug.LogErrorFormat(format, args);
	}

	public static void LogErrorFormat(global::UnityEngine.Object context, string format, params object[] args)
	{
		if (global::Debug.s_loggingDisabled)
		{
			return;
		}
		global::Debug.WriteTimeStamped(new object[]
		{
			"[ERROR]",
			(context != null) ? context.name : "null",
			string.Format(format, args)
		});
		global::UnityEngine.Debug.LogErrorFormat(context, format, args);
	}

	public static void Assert(bool condition)
	{
		if (!condition)
		{
			global::Debug.LogError("Assert failed");
			global::Debug.Break();
		}
	}

	public static void Assert(bool condition, object message)
	{
		if (!condition)
		{
			global::Debug.LogError("Assert failed: " + message);
			global::Debug.Break();
		}
	}

	public static void Assert(bool condition, object message, global::UnityEngine.Object context)
	{
		if (!condition)
		{
			global::Debug.LogError("Assert failed: " + message, context);
			global::Debug.Break();
		}
	}

	public static void DisableLogging()
	{
		global::Debug.s_loggingDisabled = true;
	}

	[Conditional("UNITY_EDITOR")]
	public static void DrawLine(Vector3 start, Vector3 end, Color color = default(Color), float duration = 0f, bool depthTest = true)
	{
		global::UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
	}

	[Conditional("UNITY_EDITOR")]
	public static void DrawRay(Vector3 start, Vector3 dir, Color color = default(Color), float duration = 0f, bool depthTest = true)
	{
		global::UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
	}

	private static bool s_loggingDisabled;
}
