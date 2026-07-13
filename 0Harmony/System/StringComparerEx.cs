using System;
using System.Runtime.CompilerServices;

namespace System
{
	internal static class StringComparerEx
	{
		[NullableContext(1)]
		public static StringComparer FromComparison(StringComparison comparisonType)
		{
			StringComparer stringComparer;
			switch (comparisonType)
			{
			case StringComparison.CurrentCulture:
				stringComparer = StringComparer.CurrentCulture;
				break;
			case StringComparison.CurrentCultureIgnoreCase:
				stringComparer = StringComparer.CurrentCultureIgnoreCase;
				break;
			case StringComparison.InvariantCulture:
				stringComparer = StringComparer.InvariantCulture;
				break;
			case StringComparison.InvariantCultureIgnoreCase:
				stringComparer = StringComparer.InvariantCultureIgnoreCase;
				break;
			case StringComparison.Ordinal:
				stringComparer = StringComparer.Ordinal;
				break;
			case StringComparison.OrdinalIgnoreCase:
				stringComparer = StringComparer.OrdinalIgnoreCase;
				break;
			default:
				throw new ArgumentException("Invalid StringComparison value", "comparisonType");
			}
			return stringComparer;
		}
	}
}
