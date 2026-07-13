using System;
using System.Runtime.CompilerServices;

namespace System
{
	internal static class ArrayEx
	{
		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] Empty<[Nullable(2)] T>()
		{
			return ArrayEx.TypeHolder<T>.Empty;
		}

		public static int MaxLength
		{
			get
			{
				return 1879048191;
			}
		}

		private static class TypeHolder<[Nullable(2)] T>
		{
			[Nullable(1)]
			public static readonly T[] Empty = new T[0];
		}
	}
}
