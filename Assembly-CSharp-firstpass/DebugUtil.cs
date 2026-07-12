using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;

public static class DebugUtil
{
	public static void Separator()
	{
		global::Debug.Log(DebugUtil.LINE);
	}

	public static void Assert(bool test)
	{
		global::Debug.Assert(test);
	}

	public static void Assert(bool test, string message)
	{
		global::Debug.Assert(test, message);
	}

	public static void Assert(bool test, string message0, string message1)
	{
		if (!test)
		{
			DebugUtil.s_errorMessageBuilder.Length = 0;
			global::Debug.Assert(test, DebugUtil.s_errorMessageBuilder.Append(message0).Append(" ").Append(message1)
				.ToString());
		}
	}

	public static void Assert(bool test, string message0, string message1, string message2)
	{
		if (!test)
		{
			DebugUtil.s_errorMessageBuilder.Length = 0;
			global::Debug.Assert(test, DebugUtil.s_errorMessageBuilder.Append(message0).Append(" ").Append(message1)
				.Append(" ")
				.Append(message2)
				.ToString());
		}
	}

	public static void AssertArgs(bool test, params object[] objs)
	{
		if (!test)
		{
			global::Debug.LogError(DebugUtil.BuildString(objs));
		}
	}

	public static string BuildString(object[] objs)
	{
		string text = "";
		if (objs.Length != 0)
		{
			text = ((objs[0] != null) ? objs[0].ToString() : "null");
			for (int i = 1; i < objs.Length; i++)
			{
				object obj = objs[i];
				text = text + " " + ((obj != null) ? obj.ToString() : "null");
			}
		}
		return text;
	}

	public static void DevAssert(bool test, string msg, global::UnityEngine.Object context = null)
	{
		if (!test)
		{
			global::Debug.LogWarning(msg, context);
		}
	}

	public static void DevAssertArgs(bool test, params object[] objs)
	{
		if (!test)
		{
			global::Debug.LogWarning(DebugUtil.BuildString(objs));
		}
	}

	[Conditional("UNITY_EDITOR")]
	public static void AssertEditorOnlyArgs(bool test, params object[] objs)
	{
		global::Debug.Assert(test, DebugUtil.BuildString(objs));
	}

	public static void DevAssertArgsWithStack(bool test, params object[] objs)
	{
		if (!test)
		{
			StackTrace stackTrace = new StackTrace(1, true);
			global::Debug.LogWarning(string.Format("{0}\n{1}", DebugUtil.BuildString(objs), stackTrace));
		}
	}

	public static void DevLogError(global::UnityEngine.Object context, string msg)
	{
		global::Debug.LogWarningFormat(context, msg, Array.Empty<object>());
	}

	public static void DevLogError(string msg)
	{
		global::Debug.LogWarningFormat(msg, Array.Empty<object>());
	}

	public static void DevLogErrorFormat(global::UnityEngine.Object context, string format, params object[] args)
	{
		global::Debug.LogWarningFormat(context, format, args);
	}

	public static void DevLogErrorFormat(string format, params object[] args)
	{
		global::Debug.LogWarningFormat(format, args);
	}

	public static void LogArgs(params object[] objs)
	{
		global::Debug.Log(DebugUtil.BuildString(objs));
	}

	public static void LogArgs(global::UnityEngine.Object context, params object[] objs)
	{
		global::Debug.Log(DebugUtil.BuildString(objs), context);
	}

	public static void LogWarningArgs(params object[] objs)
	{
		global::Debug.LogWarning(DebugUtil.BuildString(objs));
	}

	public static void LogWarningArgs(global::UnityEngine.Object context, params object[] objs)
	{
		global::Debug.LogWarning(DebugUtil.BuildString(objs), context);
	}

	public static void LogErrorArgs(params object[] objs)
	{
		global::Debug.LogError(DebugUtil.BuildString(objs));
	}

	public static void LogErrorArgs(global::UnityEngine.Object context, params object[] objs)
	{
		global::Debug.LogError(DebugUtil.BuildString(objs), context);
	}

	public static void LogException(global::UnityEngine.Object context, string errorMessage, Exception e)
	{
		DebugUtil.s_lastExceptionLogged = e;
		DebugUtil.LogErrorArgs(context, new object[]
		{
			errorMessage,
			"\n" + e.ToString()
		});
	}

	public static void LogExceptionCallstack(global::UnityEngine.Object context, string msg, string callstack, Exception e)
	{
		DebugUtil.s_lastExceptionLogged = e;
		DebugUtil.LogErrorArgs(context, new object[] { DebugUtil.START_CALLSTACK + callstack + DebugUtil.END_CALLSTACK + msg });
	}

	public static Exception RetrieveLastExceptionLogged()
	{
		Exception ex = DebugUtil.s_lastExceptionLogged;
		DebugUtil.s_lastExceptionLogged = null;
		return ex;
	}

	private static void RecursiveBuildFullName(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		DebugUtil.RecursiveBuildFullName(obj.transform.parent.gameObject);
		DebugUtil.fullNameBuilder.Append("/").Append(obj.name);
	}

	private static StringBuilder BuildFullName(GameObject obj)
	{
		DebugUtil.fullNameBuilder.Length = 0;
		DebugUtil.RecursiveBuildFullName(obj);
		return DebugUtil.fullNameBuilder.Append(" (").Append(obj.GetInstanceID()).Append(")");
	}

	public static string FullName(GameObject obj)
	{
		return DebugUtil.BuildFullName(obj).ToString();
	}

	public static string FullName(Component cmp)
	{
		return DebugUtil.BuildFullName(cmp.gameObject).Append(" (").Append(cmp.GetType())
			.Append(" ")
			.Append(cmp.GetInstanceID().ToString())
			.Append(")")
			.ToString();
	}

	[Conditional("UNITY_EDITOR")]
	public static void LogIfSelected(GameObject obj, params object[] objs)
	{
	}

	[Conditional("ENABLE_DETAILED_PROFILING")]
	public static void ProfileBegin(string str)
	{
	}

	[Conditional("ENABLE_DETAILED_PROFILING")]
	public static void ProfileBegin(string str, global::UnityEngine.Object target)
	{
	}

	[Conditional("ENABLE_DETAILED_PROFILING")]
	public static void ProfileEnd()
	{
	}

	public static KProfiler.Region ProfileRegion(string regionName, global::UnityEngine.Object profilerObj = null)
	{
		return new KProfiler.Region(regionName, profilerObj);
	}

	private static StringBuilder s_errorMessageBuilder = new StringBuilder();

	private static Exception s_lastExceptionLogged;

	public static string LINE = "-----------------------------------------------------------";

	public static string START_CALLSTACK = "~~~!";

	public static string END_CALLSTACK = "!~~~";

	private static StringBuilder fullNameBuilder = new StringBuilder();
}
