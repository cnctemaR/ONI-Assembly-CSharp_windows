using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace MonoMod
{
	internal static class MMDbgLog
	{
		static MMDbgLog()
		{
			bool flag3;
			if (!(Environment.GetEnvironmentVariable("MONOMOD_DBGLOG") == "1"))
			{
				string environmentVariable = Environment.GetEnvironmentVariable("MONOMOD_DBGLOG");
				bool? flag;
				if (environmentVariable == null)
				{
					flag = null;
				}
				else
				{
					string text = environmentVariable.ToLower(CultureInfo.InvariantCulture);
					flag = ((text != null) ? new bool?(text.Contains(MMDbgLog.Tag.ToLower(CultureInfo.InvariantCulture), StringComparison.Ordinal)) : null);
				}
				bool? flag2 = flag;
				flag3 = flag2.GetValueOrDefault();
			}
			else
			{
				flag3 = true;
			}
			if (flag3)
			{
				MMDbgLog.Start();
			}
		}

		public static void WaitForDebugger()
		{
			if (!MMDbgLog.Debugging)
			{
				MMDbgLog.Debugging = true;
				Debugger.Launch();
				Thread.Sleep(6000);
				Debugger.Break();
			}
		}

		public static void Start()
		{
			if (MMDbgLog.Writer != null)
			{
				return;
			}
			string text = Environment.GetEnvironmentVariable("MONOMOD_DBGLOG_PATH");
			if (text == "-")
			{
				MMDbgLog.Writer = Console.Out;
				return;
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "mmdbglog.txt";
			}
			text = Path.GetFullPath(Path.GetFileNameWithoutExtension(text) + "-" + MMDbgLog.Tag + Path.GetExtension(text));
			try
			{
				if (File.Exists(text))
				{
					File.Delete(text);
				}
			}
			catch
			{
			}
			try
			{
				string directoryName = Path.GetDirectoryName(text);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				MMDbgLog.Writer = new StreamWriter(new FileStream(text, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read | FileShare.Write | FileShare.Delete), Encoding.UTF8);
			}
			catch
			{
			}
		}

		public static void Log(string str)
		{
			TextWriter writer = MMDbgLog.Writer;
			if (writer == null)
			{
				return;
			}
			writer.WriteLine(str);
			writer.Flush();
		}

		public static T Log<T>(string str, T value)
		{
			TextWriter writer = MMDbgLog.Writer;
			if (writer == null)
			{
				return value;
			}
			writer.WriteLine(string.Format(CultureInfo.InvariantCulture, str, new object[] { value }));
			writer.Flush();
			return value;
		}

		public static readonly string Tag = typeof(MMDbgLog).Assembly.GetName().Name;

		public static TextWriter Writer;

		public static bool Debugging;
	}
}
