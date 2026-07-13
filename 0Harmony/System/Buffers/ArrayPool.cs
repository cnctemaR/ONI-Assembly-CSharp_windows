using System;
using System.Runtime.CompilerServices;

namespace System.Buffers
{
	[NullableContext(1)]
	[Nullable(0)]
	internal abstract class ArrayPool<[Nullable(2)] T>
	{
		public static ArrayPool<T> Shared
		{
			get
			{
				return ArrayPool<T>.s_shared;
			}
		}

		public static ArrayPool<T> Create()
		{
			return new ConfigurableArrayPool<T>();
		}

		public static ArrayPool<T> Create(int maxArrayLength, int maxArraysPerBucket)
		{
			return new ConfigurableArrayPool<T>(maxArrayLength, maxArraysPerBucket);
		}

		public abstract T[] Rent(int minimumLength);

		public abstract void Return(T[] array, bool clearArray = false);

		private static readonly TlsOverPerCoreLockedStacksArrayPool<T> s_shared = new TlsOverPerCoreLockedStacksArrayPool<T>();
	}
}
