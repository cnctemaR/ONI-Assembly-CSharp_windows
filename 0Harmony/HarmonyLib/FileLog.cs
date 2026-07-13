using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Mono.Cecil.Cil;

namespace HarmonyLib
{
	public static class FileLog
	{
		public static StreamWriter LogWriter { get; set; }

		public static string LogPath
		{
			get
			{
				object obj = FileLog.fileLock;
				string logPath;
				lock (obj)
				{
					if (!FileLog._logPathInited)
					{
						FileLog._logPathInited = true;
						string environmentVariable = Environment.GetEnvironmentVariable("HARMONY_NO_LOG");
						if (!string.IsNullOrEmpty(environmentVariable))
						{
							return null;
						}
						FileLog._logPath = Environment.GetEnvironmentVariable("HARMONY_LOG_FILE");
						if (string.IsNullOrEmpty(FileLog._logPath))
						{
							string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
							Directory.CreateDirectory(folderPath);
							FileLog._logPath = Path.Combine(folderPath, "harmony.log.txt");
						}
					}
					logPath = FileLog._logPath;
				}
				return logPath;
			}
		}

		private static string IndentString()
		{
			return new string(FileLog.indentChar, FileLog.indentLevel);
		}

		private static string CodePos(int offset)
		{
			return string.Format("IL_{0:X4}: ", offset);
		}

		public static void ChangeIndent(int delta)
		{
			object obj = FileLog.fileLock;
			lock (obj)
			{
				FileLog.indentLevel = Math.Max(0, FileLog.indentLevel + delta);
			}
		}

		public static void LogBuffered(string str)
		{
			object obj = FileLog.fileLock;
			lock (obj)
			{
				FileLog.buffer.Add(FileLog.IndentString() + str);
			}
		}

		public static void LogBuffered(List<string> strings)
		{
			object obj = FileLog.fileLock;
			lock (obj)
			{
				FileLog.buffer.AddRange(strings);
			}
		}

		public static List<string> GetBuffer(bool clear)
		{
			object obj = FileLog.fileLock;
			List<string> list2;
			lock (obj)
			{
				List<string> list = FileLog.buffer;
				if (clear)
				{
					FileLog.buffer = new List<string>();
				}
				list2 = list;
			}
			return list2;
		}

		public static void SetBuffer(List<string> buffer)
		{
			object obj = FileLog.fileLock;
			lock (obj)
			{
				FileLog.buffer = buffer;
			}
		}

		public static void FlushBuffer()
		{
			object obj = FileLog.fileLock;
			lock (obj)
			{
				if (FileLog.LogWriter != null)
				{
					foreach (string text in FileLog.buffer)
					{
						FileLog.LogWriter.WriteLine(text);
					}
					FileLog.buffer.Clear();
				}
				else if (FileLog.LogPath != null)
				{
					if (FileLog.buffer.Count > 0)
					{
						using (FileStream fileStream = new FileStream(FileLog.LogPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
						{
							using (StreamWriter streamWriter = new StreamWriter(fileStream))
							{
								foreach (string text2 in FileLog.buffer)
								{
									streamWriter.WriteLine(text2);
								}
								FileLog.buffer.Clear();
							}
						}
					}
				}
			}
		}

		public static void Log(string str)
		{
			object obj = FileLog.fileLock;
			lock (obj)
			{
				if (FileLog.LogWriter != null)
				{
					FileLog.LogWriter.WriteLine(FileLog.IndentString() + str);
				}
				else if (FileLog.LogPath != null)
				{
					using (FileStream fileStream = new FileStream(FileLog.LogPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
					{
						using (StreamWriter streamWriter = new StreamWriter(fileStream))
						{
							streamWriter.WriteLine(FileLog.IndentString() + str);
						}
					}
				}
			}
		}

		public static void LogILComment(int codePos, string comment)
		{
			FileLog.LogBuffered(string.Format("{0}// {1}", FileLog.CodePos(codePos), comment));
		}

		public static void LogIL(int codePos, global::System.Reflection.Emit.OpCode opcode)
		{
			FileLog.LogBuffered(string.Format("{0}{1}", FileLog.CodePos(codePos), opcode));
		}

		public static void LogIL(int codePos, global::System.Reflection.Emit.OpCode opcode, object arg)
		{
			string text = Emitter.FormatOperand(arg);
			string text2 = ((text.Length > 0) ? " " : "");
			string text3 = opcode.ToString();
			if (opcode.FlowControl == global::System.Reflection.Emit.FlowControl.Branch || opcode.FlowControl == global::System.Reflection.Emit.FlowControl.Cond_Branch)
			{
				text3 += " =>";
			}
			text3 = text3.PadRight(10);
			FileLog.LogBuffered(string.Format("{0}{1}{2}{3}", new object[]
			{
				FileLog.CodePos(codePos),
				text3,
				text2,
				text
			}));
		}

		internal static void LogIL(VariableDefinition variable)
		{
			FileLog.LogBuffered(string.Format("{0}Local var {1}: {2}{3}", new object[]
			{
				FileLog.CodePos(0),
				variable.Index,
				variable.VariableType.FullName,
				variable.IsPinned ? "(pinned)" : ""
			}));
		}

		public static void LogIL(int codePos, Label label)
		{
			FileLog.LogBuffered(FileLog.CodePos(codePos) + Emitter.FormatOperand(label));
		}

		public static void LogILBlockBegin(int codePos, ExceptionBlock block)
		{
			switch (block.blockType)
			{
			case ExceptionBlockType.BeginExceptionBlock:
				FileLog.LogBuffered(".try");
				FileLog.LogBuffered("{");
				FileLog.ChangeIndent(1);
				return;
			case ExceptionBlockType.BeginCatchBlock:
			{
				FileLog.LogIL(codePos, global::System.Reflection.Emit.OpCodes.Leave, new LeaveTry());
				FileLog.ChangeIndent(-1);
				FileLog.LogBuffered("} // end try");
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
				defaultInterpolatedStringHandler.AppendLiteral(".catch ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(block.catchType);
				FileLog.LogBuffered(defaultInterpolatedStringHandler.ToStringAndClear());
				FileLog.LogBuffered("{");
				FileLog.ChangeIndent(1);
				return;
			}
			case ExceptionBlockType.BeginExceptFilterBlock:
				FileLog.LogIL(codePos, global::System.Reflection.Emit.OpCodes.Leave, new LeaveTry());
				FileLog.ChangeIndent(-1);
				FileLog.LogBuffered("} // end try");
				FileLog.LogBuffered(".filter");
				FileLog.LogBuffered("{");
				FileLog.ChangeIndent(1);
				return;
			case ExceptionBlockType.BeginFaultBlock:
				FileLog.LogIL(codePos, global::System.Reflection.Emit.OpCodes.Leave, new LeaveTry());
				FileLog.ChangeIndent(-1);
				FileLog.LogBuffered("} // end try");
				FileLog.LogBuffered(".fault");
				FileLog.LogBuffered("{");
				FileLog.ChangeIndent(1);
				return;
			case ExceptionBlockType.BeginFinallyBlock:
				FileLog.LogIL(codePos, global::System.Reflection.Emit.OpCodes.Leave, new LeaveTry());
				FileLog.ChangeIndent(-1);
				FileLog.LogBuffered("} // end try");
				FileLog.LogBuffered(".finally");
				FileLog.LogBuffered("{");
				FileLog.ChangeIndent(1);
				return;
			default:
				return;
			}
		}

		public static void LogILBlockEnd(int codePos, ExceptionBlock block)
		{
			ExceptionBlockType blockType = block.blockType;
			if (blockType == ExceptionBlockType.EndExceptionBlock)
			{
				FileLog.LogIL(codePos, global::System.Reflection.Emit.OpCodes.Leave, new LeaveTry());
				FileLog.ChangeIndent(-1);
				FileLog.LogBuffered("} // end handler");
			}
		}

		public static void Debug(string str)
		{
			if (Harmony.DEBUG)
			{
				FileLog.Log(str);
			}
		}

		public static void Reset()
		{
			object obj = FileLog.fileLock;
			lock (obj)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 2);
				defaultInterpolatedStringHandler.AppendFormatted(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
				defaultInterpolatedStringHandler.AppendFormatted<char>(Path.DirectorySeparatorChar);
				defaultInterpolatedStringHandler.AppendLiteral("harmony.log.txt");
				string text = defaultInterpolatedStringHandler.ToStringAndClear();
				File.Delete(text);
			}
		}

		public unsafe static void LogBytes(long ptr, int len)
		{
			object obj = FileLog.fileLock;
			lock (obj)
			{
				byte* ptr2 = ptr;
				string text = "";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
				for (int i = 1; i <= len; i++)
				{
					if (text.Length == 0)
					{
						text = "#  ";
					}
					string text2 = text;
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 1);
					defaultInterpolatedStringHandler.AppendFormatted<byte>(*ptr2, "X2");
					defaultInterpolatedStringHandler.AppendLiteral(" ");
					text = text2 + defaultInterpolatedStringHandler.ToStringAndClear();
					if (i > 1 || len == 1)
					{
						if (i % 8 == 0 || i == len)
						{
							FileLog.Log(text);
							text = "";
						}
						else if (i % 4 == 0)
						{
							text += " ";
						}
					}
					ptr2++;
				}
				byte[] array = new byte[len];
				Marshal.Copy((IntPtr)ptr, array, 0, len);
				MD5 md = MD5.Create();
				byte[] array2 = md.ComputeHash(array);
				StringBuilder stringBuilder = new StringBuilder();
				for (int j = 0; j < array2.Length; j++)
				{
					stringBuilder.Append(array2[j].ToString("X2"));
				}
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 1);
				defaultInterpolatedStringHandler.AppendLiteral("HASH: ");
				defaultInterpolatedStringHandler.AppendFormatted<StringBuilder>(stringBuilder);
				FileLog.Log(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}

		private static readonly object fileLock = new object();

		private static bool _logPathInited;

		private static string _logPath;

		public static char indentChar = '\t';

		public static int indentLevel = 0;

		private static List<string> buffer = new List<string>();
	}
}
