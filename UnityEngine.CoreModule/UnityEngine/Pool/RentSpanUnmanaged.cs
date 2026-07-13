using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.Pool
{
	[Obsolete("Types with embedded references are not supported in this version of your compiler.", true)]
	[VisibleToOtherModules]
	internal readonly ref struct RentSpanUnmanaged<[IsUnmanaged] T> where T : struct, ValueType
	{
		public RentSpanUnmanaged(int length, bool clear = false)
		{
			int num = length * UnsafeUtility.SizeOf<T>();
			this.m_Array = ArrayPool<byte>.Shared.Rent(num);
			this.Span = MemoryMarshal.Cast<byte, T>(new Span<byte>(this.m_Array, 0, num));
			if (clear)
			{
				this.Span.Clear();
			}
		}

		public void Dispose()
		{
			ArrayPool<byte>.Shared.Return(this.m_Array, false);
		}

		public Span<T>.Enumerator GetEnumerator()
		{
			return this.Span.GetEnumerator();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Span<T>(in RentSpanUnmanaged<T> rentSpan)
		{
			return rentSpan.Span;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ReadOnlySpan<T>(in RentSpanUnmanaged<T> rentSpan)
		{
			return rentSpan.Span;
		}

		private readonly byte[] m_Array;

		public readonly Span<T> Span;
	}
}
