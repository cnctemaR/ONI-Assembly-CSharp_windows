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
		global::UnityEngine.Debug.Break();
	}

	public static void LogException(Exception exception)
	{
		global::UnityEngine.Debug.LogException(exception);
	}

	public static void Log(object obj, global::UnityEngine.Object context = null)
	{
		Console.Out.Write(string.Concat(new object[]
		{
			global::Debug.TimeStamp(),
			"[ INFO  ] ",
			obj,
			"\n"
		}));
	}

	public static void LogFormat(string format, params object[] args)
	{
		Console.Out.Write(global::Debug.TimeStamp() + "[ INFO  ] " + string.Format(format, args) + "\n");
	}

	public static void LogWarning(object obj, global::UnityEngine.Object context = null)
	{
		Console.Out.Write(string.Concat(new object[]
		{
			global::Debug.TimeStamp(),
			"[WARNING] ",
			obj,
			"\n"
		}));
	}

	public static void LogWarningFormat(string format, params object[] args)
	{
		Console.Out.Write(global::Debug.TimeStamp() + "[WARNING] " + string.Format(format, args) + "\n");
	}

	public static void LogError(object obj, global::UnityEngine.Object context = null)
	{
		Console.Out.Write(string.Concat(new object[]
		{
			global::Debug.TimeStamp(),
			"[ERROR] ",
			obj,
			"\n"
		}));
		if (context == null)
		{
			global::UnityEngine.Debug.LogError(obj);
		}
		else
		{
			global::UnityEngine.Debug.LogError(obj, context);
		}
	}

	public static void LogErrorFormat(string format, params object[] args)
	{
		Console.Out.Write(global::Debug.TimeStamp() + "[ERROR] " + string.Format(format, args) + "\n");
	}

	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool condition)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool condition, object message)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool condition, object message, global::UnityEngine.Object context)
	{
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
}
