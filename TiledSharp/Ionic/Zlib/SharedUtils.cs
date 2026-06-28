using System;
using System.IO;
using System.Text;

namespace Ionic.Zlib
{
	internal class SharedUtils
	{
		public static int URShift(int number, int bits)
		{
			return (int)((uint)number >> bits);
		}

		public static int ReadInput(TextReader sourceTextReader, byte[] target, int start, int count)
		{
			int num;
			if (target.Length == 0)
			{
				num = 0;
			}
			else
			{
				char[] array = new char[target.Length];
				int num2 = sourceTextReader.Read(array, start, count);
				if (num2 == 0)
				{
					num = -1;
				}
				else
				{
					for (int i = start; i < start + num2; i++)
					{
						target[i] = (byte)array[i];
					}
					num = num2;
				}
			}
			return num;
		}

		internal static byte[] ToByteArray(string sourceString)
		{
			return Encoding.UTF8.GetBytes(sourceString);
		}

		internal static char[] ToCharArray(byte[] byteArray)
		{
			return Encoding.UTF8.GetChars(byteArray);
		}
	}
}
