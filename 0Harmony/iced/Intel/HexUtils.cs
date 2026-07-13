using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel
{
	internal static class HexUtils
	{
		[NullableContext(1)]
		public static byte[] ToByteArray(string hexData)
		{
			if (hexData == null)
			{
				throw new ArgumentNullException("hexData");
			}
			if (hexData.Length == 0)
			{
				return Array2.Empty<byte>();
			}
			int num = 0;
			for (int i = 0; i < hexData.Length; i++)
			{
				if (hexData[i] != ' ')
				{
					num++;
				}
			}
			byte[] array = new byte[num / 2];
			int num2 = 0;
			int num3 = 0;
			for (;;)
			{
				if (num3 >= hexData.Length || !char.IsWhiteSpace(hexData[num3]))
				{
					if (num3 >= hexData.Length)
					{
						goto IL_010A;
					}
					int num4 = HexUtils.TryParseHexChar(hexData[num3++]);
					if (num4 < 0)
					{
						break;
					}
					while (num3 < hexData.Length && char.IsWhiteSpace(hexData[num3]))
					{
						num3++;
					}
					if (num3 >= hexData.Length)
					{
						goto Block_9;
					}
					int num5 = HexUtils.TryParseHexChar(hexData[num3++]);
					if (num5 < 0)
					{
						goto Block_10;
					}
					array[num2++] = (byte)((num4 << 4) | num5);
				}
				else
				{
					num3++;
				}
			}
			throw new ArgumentOutOfRangeException("hexData");
			Block_9:
			throw new ArgumentOutOfRangeException("hexData");
			Block_10:
			throw new ArgumentOutOfRangeException("hexData");
			IL_010A:
			if (num2 != array.Length)
			{
				throw new InvalidOperationException();
			}
			return array;
		}

		private static int TryParseHexChar(char c)
		{
			if ('0' <= c && c <= '9')
			{
				return (int)(c - '0');
			}
			if ('A' <= c && c <= 'F')
			{
				return (int)(c - 'A' + '\n');
			}
			if ('a' <= c && c <= 'f')
			{
				return (int)(c - 'a' + '\n');
			}
			return -1;
		}
	}
}
