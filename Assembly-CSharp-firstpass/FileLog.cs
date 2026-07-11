using System;
using System.Diagnostics;
using System.IO;

public class FileLog
{
	[Conditional("ENABLE_LOG")]
	public static void Initialize(string filename)
	{
		FileLog.instance = new FileLog(filename);
	}

	[Conditional("ENABLE_LOG")]
	public static void Shutdown()
	{
		if (FileLog.instance.writer != null)
		{
			FileLog.instance.writer.Close();
		}
		FileLog.instance = null;
	}

	private FileLog(string filename)
	{
		this.writer = new StreamWriter(filename);
		this.writer.AutoFlush = true;
	}

	[Conditional("ENABLE_LOG")]
	public static void Log(params object[] objs)
	{
		FileLog.instance.LogObjs(objs);
	}

	private void LogObjs(object[] objs)
	{
		string text = FileLog.BuildString(objs);
		this.writer.WriteLine(text);
	}

	private static string BuildString(object[] objs)
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

	private static FileLog instance;

	private StreamWriter writer;
}
