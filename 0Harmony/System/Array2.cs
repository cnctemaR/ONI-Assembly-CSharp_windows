using System;
using System.Runtime.CompilerServices;

namespace System
{
	internal static class Array2
	{
		[NullableContext(1)]
		public static T[] Empty<[Nullable(2)] T>()
		{
			return Array2.EmptyClass<T>.Empty;
		}

		private static class EmptyClass<T>
		{
			public static readonly T[] Empty = new T[0];
		}
	}
}
