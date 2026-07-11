using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;

public static class DebugUtil
{
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
			DebugUtil.errorMessageBuilder.Length = 0;
			global::Debug.Assert(test, DebugUtil.errorMessageBuilder.Append(message0).Append(" ").Append(message1)
				.ToString());
		}
	}

	public static void Assert(bool test, string message0, string message1, string message2)
	{
		if (!test)
		{
			DebugUtil.errorMessageBuilder.Length = 0;
			global::Debug.Assert(test, DebugUtil.errorMessageBuilder.Append(message0).Append(" ").Append(message1)
				.Append(" ")
				.Append(message2)
				.ToString());
		}
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

	public static void DevAssert(bool test, string msg)
	{
		if (!test)
		{
			global::Debug.LogWarning(msg);
		}
	}

	public static void DevAssertArgs(bool test, params object[] objs)
	{
		if (!test)
		{
			global::Debug.LogWarning(DebugUtil.BuildString(objs));
		}
	}

	public static void DevAssertArgsWithStack(bool test, params object[] objs)
	{
		if (!test)
		{
			StackTrace stackTrace = new StackTrace(1, true);
			string text = string.Format("{0}\n{1}", DebugUtil.BuildString(objs), stackTrace);
			global::Debug.LogWarning(text);
		}
	}

	public static void DevLogError(global::UnityEngine.Object context, string msg)
	{
		global::Debug.LogWarningFormat(context, msg, new object[0]);
	}

	public static void DevLogError(string msg)
	{
		global::Debug.LogWarningFormat(msg, new object[0]);
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
		string text = DebugUtil.BuildString(objs);
		global::Debug.Log(text);
	}

	public static void LogArgs(global::UnityEngine.Object context, params object[] objs)
	{
		string text = DebugUtil.BuildString(objs);
		global::Debug.Log(text, context);
	}

	public static void LogWarningArgs(params object[] objs)
	{
		string text = DebugUtil.BuildString(objs);
		global::Debug.LogWarning(text);
	}

	public static void LogWarningArgs(global::UnityEngine.Object context, params object[] objs)
	{
		string text = DebugUtil.BuildString(objs);
		global::Debug.LogWarning(text, context);
	}

	public static void LogErrorArgs(params object[] objs)
	{
		string text = DebugUtil.BuildString(objs);
		global::Debug.LogError(text);
	}

	public static void LogErrorArgs(global::UnityEngine.Object context, params object[] objs)
	{
		string text = DebugUtil.BuildString(objs);
		global::Debug.LogError(text, context);
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

	private static StringBuilder errorMessageBuilder = new StringBuilder();

	private static StringBuilder fullNameBuilder = new StringBuilder();
}
