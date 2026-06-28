using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
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

	public static void LogErrorParams(params object[] objs)
	{
		string text = global::Debug.BuildString(objs);
		global::Debug.LogError(text, null);
	}

	public static void LogError(object obj, global::UnityEngine.Object context = null)
	{
		if (context == null)
		{
			global::UnityEngine.Debug.LogError(global::Debug.TimeStamp() + obj);
		}
		else
		{
			global::UnityEngine.Debug.LogError(global::Debug.TimeStamp() + obj, context);
		}
	}

	public static void LogErrorFormat(string format, params object[] args)
	{
		global::UnityEngine.Debug.LogErrorFormat(global::Debug.TimeStamp() + format, args);
	}

	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool condition)
	{
		global::UnityEngine.Debug.Assert(condition);
	}

	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool condition, object message)
	{
		global::UnityEngine.Debug.Assert(condition, message);
	}

	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool condition, object message, global::UnityEngine.Object context)
	{
		global::UnityEngine.Debug.Assert(condition, message, context);
	}

	[Conditional("UNITY_EDITOR")]
	public static void AssertFormat(bool condition, string format, params object[] args)
	{
		global::UnityEngine.Debug.AssertFormat(condition, format, args);
	}

	[Conditional("UNITY_EDITOR")]
	public static void AssertFormat(bool condition, global::UnityEngine.Object context, string format, params object[] args)
	{
		global::UnityEngine.Debug.AssertFormat(condition, context, format, args);
	}

	[Conditional("UNITY_EDITOR")]
	public static void DrawLine(Vector3 start, Vector3 end, [Optional] Color color, float duration = 0f, bool depthTest = true)
	{
		global::UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
	}

	[Conditional("UNITY_EDITOR")]
	public static void DrawRay(Vector3 start, Vector3 dir, [Optional] Color color, float duration = 0f, bool depthTest = true)
	{
		global::UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
	}

	public static string BuildString(object[] objs)
	{
		string text = string.Empty;
		if (objs.Length > 0)
		{
			text = ((objs[0] == null) ? "null" : objs[0].ToString());
			for (int i = 1; i < objs.Length; i++)
			{
				object obj = objs[i];
				text = text + " " + ((obj == null) ? "null" : obj.ToString());
			}
		}
		return text;
	}
}
