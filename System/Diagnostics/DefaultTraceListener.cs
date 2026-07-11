using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Diagnostics
{
	public class DefaultTraceListener : TraceListener
	{
		public DefaultTraceListener()
			: base("Default")
		{
		}

		static DefaultTraceListener()
		{
			if (!DefaultTraceListener.OnWin32)
			{
				string environmentVariable = Environment.GetEnvironmentVariable("MONO_TRACE_LISTENER");
				if (environmentVariable != null)
				{
					string text;
					string text2;
					if (environmentVariable.StartsWith("Console.Out"))
					{
						text = "Console.Out";
						text2 = DefaultTraceListener.GetPrefix(environmentVariable, "Console.Out");
					}
					else if (environmentVariable.StartsWith("Console.Error"))
					{
						text = "Console.Error";
						text2 = DefaultTraceListener.GetPrefix(environmentVariable, "Console.Error");
					}
					else
					{
						text = environmentVariable;
						text2 = string.Empty;
					}
					DefaultTraceListener.MonoTraceFile = text;
					DefaultTraceListener.MonoTracePrefix = text2;
				}
			}
		}

		private static string GetPrefix(string var, string target)
		{
			if (var.Length > target.Length)
			{
				return var.Substring(target.Length + 1);
			}
			return string.Empty;
		}

		public bool AssertUiEnabled
		{
			get
			{
				return this.assertUiEnabled;
			}
			set
			{
				this.assertUiEnabled = value;
			}
		}

		[global::System.MonoTODO]
		public string LogFileName
		{
			get
			{
				return this.logFileName;
			}
			set
			{
				this.logFileName = value;
			}
		}

		public override void Fail(string message)
		{
			base.Fail(message);
		}

		public override void Fail(string message, string detailMessage)
		{
			base.Fail(message, detailMessage);
			if (this.ProcessUI(message, detailMessage) == DefaultTraceListener.DialogResult.Abort)
			{
				try
				{
					Thread.CurrentThread.Abort();
				}
				catch (MethodAccessException)
				{
				}
			}
			this.WriteLine(new StackTrace().ToString());
		}

		private DefaultTraceListener.DialogResult ProcessUI(string message, string detailMessage)
		{
			if (!this.AssertUiEnabled)
			{
				return DefaultTraceListener.DialogResult.None;
			}
			object obj;
			MethodInfo method;
			try
			{
				Assembly assembly = Assembly.Load("System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
				if (assembly == null)
				{
					return DefaultTraceListener.DialogResult.None;
				}
				Type type = assembly.GetType("System.Windows.Forms.MessageBoxButtons");
				obj = Enum.Parse(type, "AbortRetryIgnore");
				method = assembly.GetType("System.Windows.Forms.MessageBox").GetMethod("Show", new Type[]
				{
					typeof(string),
					typeof(string),
					type
				});
			}
			catch
			{
				return DefaultTraceListener.DialogResult.None;
			}
			if (method == null || obj == null)
			{
				return DefaultTraceListener.DialogResult.None;
			}
			string text = string.Format("Assertion Failed: {0} to quit, {1} to debug, {2} to continue", "Abort", "Retry", "Ignore");
			string text2 = string.Format("{0}{1}{2}{1}{1}{3}", new object[]
			{
				message,
				Environment.NewLine,
				detailMessage,
				new StackTrace()
			});
			string text3 = method.Invoke(null, new object[] { text2, text, obj }).ToString();
			if (text3 != null)
			{
				if (DefaultTraceListener.<>f__switch$map3 == null)
				{
					DefaultTraceListener.<>f__switch$map3 = new Dictionary<string, int>(2)
					{
						{ "Ignore", 0 },
						{ "Abort", 1 }
					};
				}
				int num;
				if (DefaultTraceListener.<>f__switch$map3.TryGetValue(text3, out num))
				{
					if (num == 0)
					{
						return DefaultTraceListener.DialogResult.Ignore;
					}
					if (num == 1)
					{
						return DefaultTraceListener.DialogResult.Abort;
					}
				}
			}
			return DefaultTraceListener.DialogResult.Retry;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WriteWindowsDebugString(string message);

		private void WriteDebugString(string message)
		{
			if (DefaultTraceListener.OnWin32)
			{
				DefaultTraceListener.WriteWindowsDebugString(message);
			}
			else
			{
				this.WriteMonoTrace(message);
			}
		}

		private void WriteMonoTrace(string message)
		{
			string monoTraceFile = DefaultTraceListener.MonoTraceFile;
			if (monoTraceFile != null)
			{
				if (DefaultTraceListener.<>f__switch$map4 == null)
				{
					DefaultTraceListener.<>f__switch$map4 = new Dictionary<string, int>(2)
					{
						{ "Console.Out", 0 },
						{ "Console.Error", 1 }
					};
				}
				int num;
				if (DefaultTraceListener.<>f__switch$map4.TryGetValue(monoTraceFile, out num))
				{
					if (num == 0)
					{
						Console.Out.Write(message);
						return;
					}
					if (num == 1)
					{
						Console.Error.Write(message);
						return;
					}
				}
			}
			this.WriteLogFile(message, DefaultTraceListener.MonoTraceFile);
		}

		private void WritePrefix()
		{
			if (!DefaultTraceListener.OnWin32)
			{
				this.WriteMonoTrace(DefaultTraceListener.MonoTracePrefix);
			}
		}

		private void WriteImpl(string message)
		{
			if (base.NeedIndent)
			{
				this.WriteIndent();
				this.WritePrefix();
			}
			this.WriteDebugString(message);
			if (Debugger.IsLogging())
			{
				Debugger.Log(0, null, message);
			}
			this.WriteLogFile(message, this.LogFileName);
		}

		private void WriteLogFile(string message, string logFile)
		{
			try
			{
				this.WriteLogFileImpl(message, logFile);
			}
			catch (MethodAccessException)
			{
			}
		}

		private void WriteLogFileImpl(string message, string logFile)
		{
			if (logFile != null && logFile.Length != 0)
			{
				FileInfo fileInfo = new FileInfo(logFile);
				StreamWriter streamWriter = null;
				try
				{
					if (fileInfo.Exists)
					{
						streamWriter = fileInfo.AppendText();
					}
					else
					{
						streamWriter = fileInfo.CreateText();
					}
				}
				catch
				{
					return;
				}
				using (streamWriter)
				{
					streamWriter.Write(message);
					streamWriter.Flush();
				}
			}
		}

		public override void Write(string message)
		{
			this.WriteImpl(message);
		}

		public override void WriteLine(string message)
		{
			string text = message + Environment.NewLine;
			this.WriteImpl(text);
			base.NeedIndent = true;
		}

		private const string ConsoleOutTrace = "Console.Out";

		private const string ConsoleErrorTrace = "Console.Error";

		private static readonly bool OnWin32 = Path.DirectorySeparatorChar == '\\';

		private static readonly string MonoTracePrefix;

		private static readonly string MonoTraceFile;

		private string logFileName;

		private bool assertUiEnabled;

		private enum DialogResult
		{
			None,
			Retry,
			Ignore,
			Abort
		}
	}
}
