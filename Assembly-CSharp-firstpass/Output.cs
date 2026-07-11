using System;
using UnityEngine;

public class Output
{
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

	public static void Log(params object[] objs)
	{
		string text = Output.BuildString(objs);
		Output.Print(text);
	}

	public static void LogWithObj(global::UnityEngine.Object obj, params object[] objs)
	{
		string text = Output.BuildString(objs);
		Output.PrintWithObj(obj, text);
	}

	public static void LogError(params object[] objs)
	{
		string text = Output.BuildString(objs);
		Output.LogError("ERROR: " + text);
	}

	public static void LogErrorWithObj(global::UnityEngine.Object obj, params object[] objs)
	{
		string text = Output.BuildString(objs);
		Output.LogErrorWithObj(obj, text);
	}

	public static void LogCriticalWarning(params object[] objs)
	{
		Output.LogWarning(objs);
	}

	public static void LogWarning(params object[] objs)
	{
		string text = Output.BuildString(objs);
		global::Debug.LogWarning(text, null);
	}

	public static void LogWarningWithObj(global::UnityEngine.Object obj, params object[] objs)
	{
		string text = Output.BuildString(objs);
		global::Debug.LogWarning(text, obj);
	}

	public static void Print(string str)
	{
		Console.Out.WriteLine(str);
	}

	private static void PrintWithObj(global::UnityEngine.Object obj, string str)
	{
		Console.Out.WriteLine(str + " : " + ((!(obj != null)) ? "<null>" : obj.name));
	}

	private static void Warn(string str)
	{
		Console.Out.WriteLine("WARNING: " + str);
	}

	private static void LogWarningWithObj(global::UnityEngine.Object obj, string str)
	{
		Console.Out.WriteLine("WARNING: " + str + " : " + ((!(obj != null)) ? "<null>" : obj.name));
	}

	public static void LogError(string str)
	{
		global::Debug.LogError(str, null);
	}

	private static void LogErrorWithObj(global::UnityEngine.Object obj, string str)
	{
		global::Debug.LogError(str, obj);
	}
}
