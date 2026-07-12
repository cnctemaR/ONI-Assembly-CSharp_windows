using System;

namespace UnityEngine.XR
{
	internal static class HashCodeHelper
	{
		public static int Combine(int hash1, int hash2)
		{
			return hash1 * 486187739 + hash2;
		}

		private const int k_HashCodeMultiplier = 486187739;
	}
}
