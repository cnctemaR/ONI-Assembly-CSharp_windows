using System;

namespace UnityEngine.XR
{
	internal static class HashCodeHelper
	{
		public static int Combine(int hash1, int hash2)
		{
			return hash1 * 486187739 + hash2;
		}

		public static int Combine(int hash1, int hash2, int hash3)
		{
			return HashCodeHelper.Combine(HashCodeHelper.Combine(hash1, hash2), hash3);
		}

		public static int Combine(int hash1, int hash2, int hash3, int hash4)
		{
			return HashCodeHelper.Combine(HashCodeHelper.Combine(hash1, hash2, hash3), hash4);
		}

		public static int Combine(int hash1, int hash2, int hash3, int hash4, int hash5)
		{
			return HashCodeHelper.Combine(HashCodeHelper.Combine(hash1, hash2, hash3, hash4), hash5);
		}

		public static int Combine(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6)
		{
			return HashCodeHelper.Combine(HashCodeHelper.Combine(hash1, hash2, hash3, hash4, hash5), hash6);
		}

		public static int Combine(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7)
		{
			return HashCodeHelper.Combine(HashCodeHelper.Combine(hash1, hash2, hash3, hash4, hash5, hash6), hash7);
		}

		public static int Combine(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7, int hash8)
		{
			return HashCodeHelper.Combine(HashCodeHelper.Combine(hash1, hash2, hash3, hash4, hash5, hash6, hash7), hash8);
		}

		private const int k_HashCodeMultiplier = 486187739;
	}
}
