using System;
using System.Runtime.InteropServices;

namespace System.Buffers
{
	public struct MemoryHandle : IDisposable
	{
		public unsafe MemoryHandle(IRetainable retainable, void* pinnedPointer = null, GCHandle handle = default(GCHandle))
		{
			this._retainable = retainable;
			this._pointer = pinnedPointer;
			this._handle = handle;
		}

		public unsafe void* PinnedPointer
		{
			get
			{
				return this._pointer;
			}
		}

		internal unsafe void AddOffset(int offset)
		{
			if (this._pointer == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.pointer);
				return;
			}
			this._pointer = (void*)((byte*)this._pointer + offset);
		}

		public void Dispose()
		{
			if (this._handle.IsAllocated)
			{
				this._handle.Free();
			}
			if (this._retainable != null)
			{
				this._retainable.Release();
				this._retainable = null;
			}
			this._pointer = null;
		}

		private IRetainable _retainable;

		private unsafe void* _pointer;

		private GCHandle _handle;
	}
}
