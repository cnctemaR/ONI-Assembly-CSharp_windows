using System;
using System.Diagnostics;
using UnityEngine;

public static class DebugUtil
{
	public static void Assert(bool test, string message = "Assert!")
	{
		if (!test)
		{
			Output.LogError(new object[] { message });
			global::Debug.Break();
		}
	}

	public static void DevAssert(bool test, string message = "Assert!")
	{
		if (!test)
		{
			if (Application.isEditor)
			{
				Output.LogError(new object[] { message });
				global::Debug.Break();
			}
			else
			{
				Output.LogWarning(new object[] { message });
			}
		}
	}

	public static void SoftAssert(bool test, string message = "Assert!")
	{
		if (!test)
		{
			global::Debug.LogWarning(message, null);
		}
	}

	public static string FullName(Component cmp)
	{
		return string.Concat(new object[]
		{
			DebugUtil.FullName(cmp.gameObject),
			" (",
			cmp.GetType().ToString(),
			" ",
			cmp.GetInstanceID(),
			")"
		});
	}

	public static string FullName(GameObject obj)
	{
		GameObject gameObject = obj;
		string text = "/" + obj.name;
		while (obj.transform.parent != null)
		{
			obj = obj.transform.parent.gameObject;
			text = "/" + obj.name + text;
		}
		return string.Concat(new object[]
		{
			text,
			" (",
			gameObject.GetInstanceID(),
			")"
		});
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
}
