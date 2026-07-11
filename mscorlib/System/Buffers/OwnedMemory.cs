using System;

namespace System.Buffers
{
	public abstract class OwnedMemory<T> : IDisposable, IRetainable
	{
		public abstract int Length { get; }

		public abstract Span<T> Span { get; }

		public Memory<T> Memory
		{
			get
			{
				if (this.IsDisposed)
				{
					ThrowHelper.ThrowObjectDisposedException_MemoryDisposed("OwnedMemory");
				}
				return new Memory<T>(this, 0, this.Length);
			}
		}

		public abstract MemoryHandle Pin();

		protected internal abstract bool TryGetArray(out ArraySegment<T> arraySegment);

		public void Dispose()
		{
			if (this.IsRetained)
			{
				ThrowHelper.ThrowInvalidOperationException_OutstandingReferences();
			}
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected abstract void Dispose(bool disposing);

		protected abstract bool IsRetained { get; }

		public abstract bool IsDisposed { get; }

		public abstract void Retain();

		public abstract bool Release();
	}
}
