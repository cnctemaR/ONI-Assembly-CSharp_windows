using System;

namespace System.Xml
{
	internal static class Bits
	{
		public static int Count(uint num)
		{
			num = (num & Bits.MASK_0101010101010101) + ((num >> 1) & Bits.MASK_0101010101010101);
			num = (num & Bits.MASK_0011001100110011) + ((num >> 2) & Bits.MASK_0011001100110011);
			num = (num & Bits.MASK_0000111100001111) + ((num >> 4) & Bits.MASK_0000111100001111);
			num = (num & Bits.MASK_0000000011111111) + ((num >> 8) & Bits.MASK_0000000011111111);
			num = (num & Bits.MASK_1111111111111111) + (num >> 16);
			return (int)num;
		}

		public static bool ExactlyOne(uint num)
		{
			return num != 0U && (num & (num - 1U)) == 0U;
		}

		public static bool MoreThanOne(uint num)
		{
			return (num & (num - 1U)) > 0U;
		}

		public static uint ClearLeast(uint num)
		{
			return num & (num - 1U);
		}

		public static int LeastPosition(uint num)
		{
			if (num == 0U)
			{
				return 0;
			}
			return Bits.Count(num ^ (num - 1U));
		}

		private static readonly uint MASK_0101010101010101 = 1431655765U;

		private static readonly uint MASK_0011001100110011 = 858993459U;

		private static readonly uint MASK_0000111100001111 = 252645135U;

		private static readonly uint MASK_0000000011111111 = 16711935U;

		private static readonly uint MASK_1111111111111111 = 65535U;
	}
}
