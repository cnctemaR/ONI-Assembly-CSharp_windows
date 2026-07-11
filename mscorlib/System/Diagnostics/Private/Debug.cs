using System;
using Internal.Runtime.Augments;

namespace System.Diagnostics.Private
{
	internal static class Debug
	{
		public static bool AutoFlush
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		public static int IndentLevel
		{
			get
			{
				return Debug.s_indentLevel;
			}
			set
			{
				Debug.s_indentLevel = ((value < 0) ? 0 : value);
			}
		}

		public static int IndentSize
		{
			get
			{
				return Debug.s_indentSize;
			}
			set
			{
				Debug.s_indentSize = ((value < 0) ? 0 : value);
			}
		}

		[Conditional("DEBUG")]
		public static void Close()
		{
		}

		[Conditional("DEBUG")]
		public static void Flush()
		{
		}

		[Conditional("DEBUG")]
		public static void Indent()
		{
			Debug.IndentLevel++;
		}

		[Conditional("DEBUG")]
		public static void Unindent()
		{
			Debug.IndentLevel--;
		}

		[Conditional("DEBUG")]
		public static void Print(string message)
		{
			Debug.Write(message);
		}

		[Conditional("DEBUG")]
		public static void Print(string format, params object[] args)
		{
			Debug.Write(string.Format(null, format, args));
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition)
		{
			Debug.Assert(condition, string.Empty, string.Empty);
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition, string message)
		{
			Debug.Assert(condition, message, string.Empty);
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition, string message, string detailMessage)
		{
			if (!condition)
			{
				string text;
				try
				{
					text = EnvironmentAugments.StackTrace;
				}
				catch
				{
					text = "";
				}
				Debug.WriteLine(Debug.FormatAssert(text, message, detailMessage));
				Debug.s_ShowAssertDialog(text, message, detailMessage);
			}
		}

		[Conditional("DEBUG")]
		public static void Fail(string message)
		{
			Debug.Assert(false, message, string.Empty);
		}

		[Conditional("DEBUG")]
		public static void Fail(string message, string detailMessage)
		{
			Debug.Assert(false, message, detailMessage);
		}

		private static string FormatAssert(string stackTrace, string message, string detailMessage)
		{
			string text = Debug.GetIndentString() + Environment.NewLine;
			return string.Concat(new string[]
			{
				"---- DEBUG ASSERTION FAILED ----", text, "---- Assert Short Message ----", text, message, text, "---- Assert Long Message ----", text, detailMessage, text,
				stackTrace
			});
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition, string message, string detailMessageFormat, params object[] args)
		{
			Debug.Assert(condition, message, string.Format(detailMessageFormat, args));
		}

		[Conditional("DEBUG")]
		public static void WriteLine(string message)
		{
			Debug.Write(message + Environment.NewLine);
		}

		[Conditional("DEBUG")]
		public static void Write(string message)
		{
			object obj = Debug.s_lock;
			lock (obj)
			{
				if (message == null)
				{
					Debug.s_WriteCore(string.Empty);
				}
				else
				{
					if (Debug.s_needIndent)
					{
						message = Debug.GetIndentString() + message;
						Debug.s_needIndent = false;
					}
					Debug.s_WriteCore(message);
					if (message.EndsWith(Environment.NewLine))
					{
						Debug.s_needIndent = true;
					}
				}
			}
		}

		[Conditional("DEBUG")]
		public static void WriteLine(object value)
		{
			Debug.WriteLine((value != null) ? value.ToString() : null);
		}

		[Conditional("DEBUG")]
		public static void WriteLine(object value, string category)
		{
			Debug.WriteLine((value != null) ? value.ToString() : null, category);
		}

		[Conditional("DEBUG")]
		public static void WriteLine(string format, params object[] args)
		{
			Debug.WriteLine(string.Format(null, format, args));
		}

		[Conditional("DEBUG")]
		public static void WriteLine(string message, string category)
		{
			if (category == null)
			{
				Debug.WriteLine(message);
				return;
			}
			Debug.WriteLine(category + ":" + message);
		}

		[Conditional("DEBUG")]
		public static void Write(object value)
		{
			Debug.Write((value != null) ? value.ToString() : null);
		}

		[Conditional("DEBUG")]
		public static void Write(string message, string category)
		{
			if (category == null)
			{
				Debug.Write(message);
				return;
			}
			Debug.Write(category + ":" + message);
		}

		[Conditional("DEBUG")]
		public static void Write(object value, string category)
		{
			Debug.Write((value != null) ? value.ToString() : null, category);
		}

		[Conditional("DEBUG")]
		public static void WriteIf(bool condition, string message)
		{
			if (condition)
			{
				Debug.Write(message);
			}
		}

		[Conditional("DEBUG")]
		public static void WriteIf(bool condition, object value)
		{
			if (condition)
			{
				Debug.Write(value);
			}
		}

		[Conditional("DEBUG")]
		public static void WriteIf(bool condition, string message, string category)
		{
			if (condition)
			{
				Debug.Write(message, category);
			}
		}

		[Conditional("DEBUG")]
		public static void WriteIf(bool condition, object value, string category)
		{
			if (condition)
			{
				Debug.Write(value, category);
			}
		}

		[Conditional("DEBUG")]
		public static void WriteLineIf(bool condition, object value)
		{
			if (condition)
			{
				Debug.WriteLine(value);
			}
		}

		[Conditional("DEBUG")]
		public static void WriteLineIf(bool condition, object value, string category)
		{
			if (condition)
			{
				Debug.WriteLine(value, category);
			}
		}

		[Conditional("DEBUG")]
		public static void WriteLineIf(bool condition, string message)
		{
			if (condition)
			{
				Debug.WriteLine(message);
			}
		}

		[Conditional("DEBUG")]
		public static void WriteLineIf(bool condition, string message, string category)
		{
			if (condition)
			{
				Debug.WriteLine(message, category);
			}
		}

		private static string GetIndentString()
		{
			int num = Debug.IndentSize * Debug.IndentLevel;
			string text = Debug.s_indentString;
			if (text != null && text.Length == num)
			{
				return Debug.s_indentString;
			}
			return Debug.s_indentString = new string(' ', num);
		}

		private static void ShowAssertDialog(string stackTrace, string message, string detailMessage)
		{
		}

		private static void WriteCore(string message)
		{
		}

		private static readonly object s_lock = new object();

		[ThreadStatic]
		private static int s_indentLevel;

		private static int s_indentSize = 4;

		private static bool s_needIndent;

		private static string s_indentString;

		internal static Action<string, string, string> s_ShowAssertDialog = new Action<string, string, string>(Debug.ShowAssertDialog);

		internal static Action<string> s_WriteCore = new Action<string>(Debug.WriteCore);

		private sealed class DebugAssertException : Exception
		{
			internal DebugAssertException(string message, string detailMessage, string stackTrace)
				: base(string.Concat(new string[]
				{
					message,
					Environment.NewLine,
					detailMessage,
					Environment.NewLine,
					stackTrace
				}))
			{
			}
		}
	}
}
