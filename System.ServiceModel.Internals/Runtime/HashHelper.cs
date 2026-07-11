using System;

namespace System.Runtime
{
	internal static class HashHelper
	{
		public static byte[] ComputeHash(byte[] buffer)
		{
			int[] array = new int[]
			{
				7, 12, 17, 22, 5, 9, 14, 20, 4, 11,
				16, 23, 6, 10, 15, 21
			};
			uint[] array2 = new uint[]
			{
				3614090360U, 3905402710U, 606105819U, 3250441966U, 4118548399U, 1200080426U, 2821735955U, 4249261313U, 1770035416U, 2336552879U,
				4294925233U, 2304563134U, 1804603682U, 4254626195U, 2792965006U, 1236535329U, 4129170786U, 3225465664U, 643717713U, 3921069994U,
				3593408605U, 38016083U, 3634488961U, 3889429448U, 568446438U, 3275163606U, 4107603335U, 1163531501U, 2850285829U, 4243563512U,
				1735328473U, 2368359562U, 4294588738U, 2272392833U, 1839030562U, 4259657740U, 2763975236U, 1272893353U, 4139469664U, 3200236656U,
				681279174U, 3936430074U, 3572445317U, 76029189U, 3654602809U, 3873151461U, 530742520U, 3299628645U, 4096336452U, 1126891415U,
				2878612391U, 4237533241U, 1700485571U, 2399980690U, 4293915773U, 2240044497U, 1873313359U, 4264355552U, 2734768916U, 1309151649U,
				4149444226U, 3174756917U, 718787259U, 3951481745U
			};
			int num = (buffer.Length + 8) / 64 + 1;
			uint num2 = 1732584193U;
			uint num3 = 4023233417U;
			uint num4 = 2562383102U;
			uint num5 = 271733878U;
			for (int i = 0; i < num; i++)
			{
				byte[] array3 = buffer;
				int num6 = i * 64;
				if (num6 + 64 > buffer.Length)
				{
					array3 = new byte[64];
					for (int j = num6; j < buffer.Length; j++)
					{
						array3[j - num6] = buffer[j];
					}
					if (num6 <= buffer.Length)
					{
						array3[buffer.Length - num6] = 128;
					}
					if (i == num - 1)
					{
						array3[56] = (byte)(buffer.Length << 3);
						array3[57] = (byte)(buffer.Length >> 5);
						array3[58] = (byte)(buffer.Length >> 13);
						array3[59] = (byte)(buffer.Length >> 21);
					}
					num6 = 0;
				}
				uint num7 = num2;
				uint num8 = num3;
				uint num9 = num4;
				uint num10 = num5;
				for (int k = 0; k < 64; k++)
				{
					uint num11;
					int num12;
					if (k < 16)
					{
						num11 = (num8 & num9) | (~num8 & num10);
						num12 = k;
					}
					else if (k < 32)
					{
						num11 = (num8 & num10) | (num9 & ~num10);
						num12 = 5 * k + 1;
					}
					else if (k < 48)
					{
						num11 = num8 ^ num9 ^ num10;
						num12 = 3 * k + 5;
					}
					else
					{
						num11 = num9 ^ (num8 | ~num10);
						num12 = 7 * k;
					}
					num12 = (num12 & 15) * 4 + num6;
					uint num13 = num10;
					num10 = num9;
					num9 = num8;
					num8 = num7 + num11 + array2[k] + (uint)((int)array3[num12] + ((int)array3[num12 + 1] << 8) + ((int)array3[num12 + 2] << 16) + ((int)array3[num12 + 3] << 24));
					num8 = (num8 << array[(k & 3) | ((k >> 2) & -4)]) | (num8 >> 32 - array[(k & 3) | ((k >> 2) & -4)]);
					num8 += num9;
					num7 = num13;
				}
				num2 += num7;
				num3 += num8;
				num4 += num9;
				num5 += num10;
			}
			return new byte[]
			{
				(byte)num2,
				(byte)(num2 >> 8),
				(byte)(num2 >> 16),
				(byte)(num2 >> 24),
				(byte)num3,
				(byte)(num3 >> 8),
				(byte)(num3 >> 16),
				(byte)(num3 >> 24),
				(byte)num4,
				(byte)(num4 >> 8),
				(byte)(num4 >> 16),
				(byte)(num4 >> 24),
				(byte)num5,
				(byte)(num5 >> 8),
				(byte)(num5 >> 16),
				(byte)(num5 >> 24)
			};
		}
	}
}
