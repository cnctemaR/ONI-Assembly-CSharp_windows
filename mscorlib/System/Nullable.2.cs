using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	public static class Nullable
	{
		public static int Compare<T>(T? value1, T? value2) where T : struct
		{
			if (!value1.has_value)
			{
				return (!value2.has_value) ? 0 : (-1);
			}
			if (!value2.has_value)
			{
				return 1;
			}
			return Comparer<T>.Default.Compare(value1.value, value2.value);
		}

		public static bool Equals<T>(T? value1, T? value2) where T : struct
		{
			return value1.has_value == value2.has_value && (!value1.has_value || EqualityComparer<T>.Default.Equals(value1.value, value2.value));
		}

		public static Type GetUnderlyingType(Type nullableType)
		{
			if (nullableType == null)
			{
				throw new ArgumentNullException("nullableType");
			}
			if (nullableType.IsGenericType && nullableType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				return nullableType.GetGenericArguments()[0];
			}
			return null;
		}
	}
}
