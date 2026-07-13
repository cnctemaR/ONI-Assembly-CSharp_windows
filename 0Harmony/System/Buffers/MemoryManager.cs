using System;
using System.Runtime.CompilerServices;

namespace System.Buffers
{
	internal abstract class MemoryManager<[Nullable(2)] T> : IMemoryOwner<T>, IDisposable, IPinnable
	{
		[Nullable(new byte[] { 0, 1 })]
		public virtual global::System.Memory<T> Memory
		{
			[return: Nullable(new byte[] { 0, 1 })]
			get
			{
				return new global::System.Memory<T>(this, this.GetSpan().Length);
			}
		}

		[return: Nullable(new byte[] { 0, 1 })]
		public abstract global::System.Span<T> GetSpan();

		public abstract MemoryHandle Pin(int elementIndex = 0);

		public abstract void Unpin();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: Nullable(new byte[] { 0, 1 })]
		protected global::System.Memory<T> CreateMemory(int length)
		{
			return new global::System.Memory<T>(this, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: Nullable(new byte[] { 0, 1 })]
		protected global::System.Memory<T> CreateMemory(int start, int length)
		{
			return new global::System.Memory<T>(this, start, length);
		}

		protected internal virtual bool TryGetArray([Nullable(new byte[] { 0, 1 })] out ArraySegment<T> segment)
		{
			segment = default(ArraySegment<T>);
			return false;
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected abstract void Dispose(bool disposing);
	}
}
