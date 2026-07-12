using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	internal static class HashCodeCalculator
	{
		public static int Calculate<T>(ICollection<T> list)
		{
			if (list == null)
			{
				return 0;
			}
			int num = 17;
			foreach (T t in list)
			{
				num = num * 29 + t.GetHashCode();
			}
			return num;
		}
	}
}
