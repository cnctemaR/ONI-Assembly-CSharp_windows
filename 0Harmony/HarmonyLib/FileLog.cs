using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace HarmonyLib
{
	public static class FileLog
	{
		static FileLog()
		{
			string environmentVariable = Environment.GetEnvironmentVariable("HARMONY_LOG_FILE");
			if (!string.IsNullOrEmpty(environmentVariable))
			{
				FileLog.logPath = environmentVariable;
				return;
			}
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			Directory.CreateDirectory(folderPath);
			FileLog.logPath = Path.Combine(folderPath, "harmony.log.txt");
		}

		private static string IndentString()
		{
			return new string(FileLog.indentChar, FileLog.indentLevel);
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
				if (FileLog.buffer.Count > 0)
				{
					using (StreamWriter streamWriter = File.AppendText(FileLog.logPath))
					{
						foreach (string text in FileLog.buffer)
						{
							streamWriter.WriteLine(text);
						}
					}
					FileLog.buffer.Clear();
				}
			}
		}

		public static void Log(string str)
		{
			object obj = FileLog.fileLock;
			lock (obj)
			{
				using (StreamWriter streamWriter = File.AppendText(FileLog.logPath))
				{
					streamWriter.WriteLine(FileLog.IndentString() + str);
				}
			}
		}

		public static void Reset()
		{
			object obj = FileLog.fileLock;
			lock (obj)
			{
				File.Delete(string.Format("{0}{1}harmony.log.txt", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Path.DirectorySeparatorChar));
			}
		}

		public unsafe static void LogBytes(long ptr, int len)
		{
			object obj = FileLog.fileLock;
			lock (obj)
			{
				byte* ptr2 = ptr;
				string text = "";
				for (int i = 1; i <= len; i++)
				{
					if (text.Length == 0)
					{
						text = "#  ";
					}
					text += string.Format("{0:X2} ", *ptr2);
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
				byte[] array2 = MD5.Create().ComputeHash(array);
				StringBuilder stringBuilder = new StringBuilder();
				for (int j = 0; j < array2.Length; j++)
				{
					stringBuilder.Append(array2[j].ToString("X2"));
				}
				FileLog.Log(string.Format("HASH: {0}", stringBuilder));
			}
		}

		private static readonly object fileLock = new object();

		public static string logPath;

		public static char indentChar = '\t';

		public static int indentLevel = 0;

		private static List<string> buffer = new List<string>();
	}
}
