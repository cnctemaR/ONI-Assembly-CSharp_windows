using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Pool
{
	[VisibleToOtherModules]
	internal readonly struct RentMemory<T> : IDisposable where T : class
	{
		public RentMemory(int length, bool clear = false)
		{
			this.m_Array = ArrayPool<T>.Shared.Rent(length);
			this.Memory = this.m_Array.AsMemory<T>(0, length);
			if (clear)
			{
				this.Memory.Span.Clear();
			}
		}

		public Span<T>.Enumerator GetEnumerator()
		{
			return this.Memory.Span.GetEnumerator();
		}

		public void Dispose()
		{
			ArrayPool<T>.Shared.Return(this.m_Array, false);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Memory<T>(in RentMemory<T> rentMemory)
		{
			return rentMemory.Memory;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ReadOnlyMemory<T>(in RentMemory<T> rentMemory)
		{
			return rentMemory.Memory;
		}

		private readonly T[] m_Array;

		public readonly Memory<T> Memory;
	}
}
