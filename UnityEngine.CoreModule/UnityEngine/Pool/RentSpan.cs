using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Pool
{
	[VisibleToOtherModules]
	[Obsolete("Types with embedded references are not supported in this version of your compiler.", true)]
	internal readonly ref struct RentSpan<T> where T : class
	{
		public RentSpan(int length, bool clear = false)
		{
			this.m_Array = ArrayPool<T>.Shared.Rent(length);
			this.Span = this.m_Array.AsSpan<T>(0, length);
			if (clear)
			{
				this.Span.Clear();
			}
		}

		public void Dispose()
		{
			ArrayPool<T>.Shared.Return(this.m_Array, false);
		}

		public Span<T>.Enumerator GetEnumerator()
		{
			return this.Span.GetEnumerator();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Span<T>(in RentSpan<T> rentSpan)
		{
			return rentSpan.Span;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ReadOnlySpan<T>(in RentSpan<T> rentSpan)
		{
			return rentSpan.Span;
		}

		public Memory<T> AsMemory()
		{
			return new Memory<T>(this.m_Array, 0, this.Span.Length);
		}

		private readonly T[] m_Array;

		public readonly Span<T> Span;
	}
}
