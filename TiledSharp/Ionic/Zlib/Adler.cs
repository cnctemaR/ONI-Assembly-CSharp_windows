using System;

namespace Ionic.Zlib
{
	public sealed class Adler
	{
		public static uint Adler32(uint adler, byte[] buf, int index, int len)
		{
			uint num;
			if (buf == null)
			{
				num = 1U;
			}
			else
			{
				uint num2 = adler & 65535U;
				uint num3 = (adler >> 16) & 65535U;
				while (len > 0)
				{
					int i = ((len < Adler.NMAX) ? len : Adler.NMAX);
					len -= i;
					while (i >= 16)
					{
						num2 += (uint)buf[index++];
						num3 += num2;
						num2 += (uint)buf[index++];
						num3 += num2;
						num2 += (uint)buf[index++];
						num3 += num2;
						num2 += (uint)buf[index++];
						num3 += num2;
						num2 += (uint)buf[index++];
						num3 += num2;
						num2 += (uint)buf[index++];
						num3 += num2;
						num2 += (uint)buf[index++];
						num3 += num2;
						num2 += (uint)buf[index++];
						num3 += num2;
						num2 += (uint)buf[index++];
						num3 += num2;
						num2 += (uint)buf[index++];
						num3 += num2;
						num2 += (uint)buf[index++];
						num3 += num2;
						num2 += (uint)buf[index++];
						num3 += num2;
						num2 += (uint)buf[index++];
						num3 += num2;
						num2 += (uint)buf[index++];
						num3 += num2;
						num2 += (uint)buf[index++];
						num3 += num2;
						num2 += (uint)buf[index++];
						num3 += num2;
						i -= 16;
					}
					if (i != 0)
					{
						do
						{
							num2 += (uint)buf[index++];
							num3 += num2;
						}
						while (--i != 0);
					}
					num2 %= Adler.BASE;
					num3 %= Adler.BASE;
				}
				num = (num3 << 16) | num2;
			}
			return num;
		}

		private static readonly uint BASE = 65521U;

		private static readonly int NMAX = 5552;
	}
}
