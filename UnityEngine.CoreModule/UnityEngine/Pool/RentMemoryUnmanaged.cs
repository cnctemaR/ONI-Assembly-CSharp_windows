using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Pool
{
	[VisibleToOtherModules]
	internal readonly struct RentMemoryUnmanaged<[IsUnmanaged] T> : IDisposable where T : struct, ValueType
	{
		public RentMemoryUnmanaged(int length, bool clear = false)
		{
			this.m_Array = ArrayPool<T>.Shared.Rent(length);
			this.Memory = new Memory<T>(this.m_Array, 0, length);
			if (clear)
			{
				this.Memory.Span.Clear();
			}
		}

		public void Dispose()
		{
			ArrayPool<T>.Shared.Return(this.m_Array, false);
		}

		public Span<T>.Enumerator GetEnumerator()
		{
			return this.Memory.Span.GetEnumerator();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Memory<T>(in RentMemoryUnmanaged<T> rentMemory)
		{
			return rentMemory.Memory;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ReadOnlyMemory<T>(in RentMemoryUnmanaged<T> rentMemory)
		{
			return rentMemory.Memory;
		}

		private readonly T[] m_Array;

		public readonly Memory<T> Memory;
	}
}
