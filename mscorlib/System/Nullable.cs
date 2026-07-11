using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	public static class Nullable
	{
		public static int Compare<T>(T? n1, T? n2) where T : struct
		{
			if (n1.has_value)
			{
				if (!n2.has_value)
				{
					return 1;
				}
				return Comparer<T>.Default.Compare(n1.value, n2.value);
			}
			else
			{
				if (!n2.has_value)
				{
					return 0;
				}
				return -1;
			}
		}

		public static bool Equals<T>(T? n1, T? n2) where T : struct
		{
			return n1.has_value == n2.has_value && (!n1.has_value || EqualityComparer<T>.Default.Equals(n1.value, n2.value));
		}

		public static Type GetUnderlyingType(Type nullableType)
		{
			if (nullableType == null)
			{
				throw new ArgumentNullException("nullableType");
			}
			if (!nullableType.IsGenericType || nullableType.IsGenericTypeDefinition || !(nullableType.GetGenericTypeDefinition() == typeof(Nullable<>)))
			{
				return null;
			}
			return nullableType.GetGenericArguments()[0];
		}
	}
}
