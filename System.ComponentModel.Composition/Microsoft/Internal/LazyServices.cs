using System;
using System.Globalization;

namespace Microsoft.Internal
{
	internal static class LazyServices
	{
		public static T GetNotNullValue<T>(this Lazy<T> lazy, string argument) where T : class
		{
			Assumes.NotNull<Lazy<T>>(lazy);
			T value = lazy.Value;
			if (value == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.LazyServices_LazyResolvesToNull, typeof(T), argument));
			}
			return value;
		}
	}
}
