using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Harmony
{
	public static class FileLog
	{
		private static string IndentString()
		{
			return new string(FileLog.indentChar, FileLog.indentLevel);
		}

		public static void ChangeIndent(int delta)
		{
			FileLog.indentLevel = Math.Max(0, FileLog.indentLevel + delta);
		}

		public static void LogBuffered(string str)
		{
			string text = FileLog.logPath;
			lock (text)
			{
				FileLog.buffer.Add(FileLog.IndentString() + str);
			}
		}

		public static void FlushBuffer()
		{
			string text = FileLog.logPath;
			lock (text)
			{
				bool flag = FileLog.buffer.Count > 0;
				if (flag)
				{
					using (StreamWriter streamWriter = File.AppendText(FileLog.logPath))
					{
						foreach (string text2 in FileLog.buffer)
						{
							streamWriter.WriteLine(text2);
						}
					}
					FileLog.buffer.Clear();
				}
			}
		}

		public static void Log(string str)
		{
			string text = FileLog.logPath;
			lock (text)
			{
				using (StreamWriter streamWriter = File.AppendText(FileLog.logPath))
				{
					streamWriter.WriteLine(FileLog.IndentString() + str);
				}
			}
		}

		public static void Reset()
		{
			string text = FileLog.logPath;
			lock (text)
			{
				string text2 = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + Path.DirectorySeparatorChar.ToString() + "harmony.log.txt";
				File.Delete(text2);
			}
		}

		public unsafe static void LogBytes(long ptr, int len)
		{
			string text = FileLog.logPath;
			lock (text)
			{
				byte* ptr2 = ptr;
				string text2 = "";
				for (int i = 1; i <= len; i++)
				{
					bool flag = text2 == "";
					if (flag)
					{
						text2 = "#  ";
					}
					text2 = text2 + ptr2->ToString("X2") + " ";
					bool flag2 = i > 1 || len == 1;
					if (flag2)
					{
						bool flag3 = i % 8 == 0 || i == len;
						if (flag3)
						{
							FileLog.Log(text2);
							text2 = "";
						}
						else
						{
							bool flag4 = i % 4 == 0;
							if (flag4)
							{
								text2 += " ";
							}
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
				FileLog.Log("HASH: " + stringBuilder);
			}
		}

		public static string logPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + Path.DirectorySeparatorChar.ToString() + "harmony.log.txt";

		public static char indentChar = '\t';

		public static int indentLevel = 0;

		private static List<string> buffer = new List<string>();
	}
}
