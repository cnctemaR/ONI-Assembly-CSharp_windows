using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal static class IEnumerableExtensions
	{
		internal static bool HasValues(this IEnumerable<string> collection)
		{
			bool flag = collection == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				using (IEnumerator<string> enumerator = collection.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						string text = enumerator.Current;
						return true;
					}
				}
				flag2 = false;
			}
			return flag2;
		}
	}
}
