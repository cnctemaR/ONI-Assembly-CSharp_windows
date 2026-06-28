using System;
using System.IO;
using System.Text;

namespace FileHelpers
{
	internal static class StreamHelper
	{
		internal static TextWriter CreateFileAppender(string fileName, Encoding encode, bool correctEnd, bool disposeStream, int bufferSize)
		{
			if (correctEnd)
			{
				FileStream fileStream = null;
				try
				{
					fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
					long num;
					for (num = fileStream.Length - 1L; num >= 0L; num -= 1L)
					{
						fileStream.Seek(num, SeekOrigin.Begin);
						int num2 = fileStream.ReadByte();
						if (num2 != 13 && num2 != 10)
						{
							break;
						}
					}
					if (num >= 0L)
					{
						byte[] array = new byte[StringHelper.NewLine.Length];
						int num3 = 0;
						foreach (char c in StringHelper.NewLine)
						{
							array[num3] = Convert.ToByte(c);
							num3++;
						}
						fileStream.Write(array, 0, num3);
					}
					return new StreamWriter(fileStream, encode, bufferSize);
				}
				finally
				{
					if (disposeStream && fileStream != null)
					{
						fileStream.Close();
					}
				}
			}
			return new StreamWriter(fileName, true, encode, bufferSize);
		}
	}
}
