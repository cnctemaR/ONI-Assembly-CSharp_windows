using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;

public static class DebugUtil
{
	private static void Break(string message)
	{
		global::Debug.LogError(message, null);
		global::Debug.Break();
		Debugger.Break();
	}

	public static void Assert(bool test)
	{
		if (!test)
		{
			DebugUtil.Break("Failed assertion");
		}
	}

	public static void Assert(bool test, string message)
	{
		if (!test)
		{
			DebugUtil.Break(message);
		}
	}

	public static void Assert(bool test, string message0, string message1)
	{
		if (!test)
		{
			DebugUtil.errorMessageBuilder.Length = 0;
			DebugUtil.Break(DebugUtil.errorMessageBuilder.Append(message0).Append(" ").Append(message1)
				.ToString());
		}
	}

	public static void Assert(bool test, string message0, string message1, string message2)
	{
		if (!test)
		{
			DebugUtil.errorMessageBuilder.Length = 0;
			DebugUtil.Break(DebugUtil.errorMessageBuilder.Append(message0).Append(" ").Append(message1)
				.Append(" ")
				.Append(message2)
				.ToString());
		}
	}

	public static void Assert(bool test, params object[] objs)
	{
		if (!test)
		{
			global::Debug.LogError(Output.BuildString(objs), null);
			global::Debug.Break();
			Debugger.Break();
		}
	}

	public static void DevAssert(bool test, params object[] objs)
	{
		if (!test)
		{
			if (Application.isEditor)
			{
				global::Debug.LogError(Output.BuildString(objs), null);
				global::Debug.Break();
				Debugger.Break();
			}
			else
			{
				global::Debug.LogWarning(Output.BuildString(objs), null);
			}
		}
	}

	public static void DevAssertWithStack(bool test, params object[] objs)
	{
		if (!test)
		{
			if (Application.isEditor)
			{
				global::Debug.LogError(Output.BuildString(objs), null);
				global::Debug.Break();
				Debugger.Break();
			}
			else
			{
				StackTrace stackTrace = new StackTrace(1, true);
				string text = string.Format("{0}\n{1}", Output.BuildString(objs), stackTrace);
				global::Debug.LogWarning(text, null);
			}
		}
	}

	public static void SoftAssert(bool test, params object[] objs)
	{
		if (!test)
		{
			global::Debug.LogWarning(Output.BuildString(objs), null);
		}
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
