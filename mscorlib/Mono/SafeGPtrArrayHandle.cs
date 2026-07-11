using System;

namespace Mono
{
	internal struct SafeGPtrArrayHandle : IDisposable
	{
		internal SafeGPtrArrayHandle(IntPtr ptr)
		{
			this.handle = new RuntimeGPtrArrayHandle(ptr);
		}

		public void Dispose()
		{
			RuntimeGPtrArrayHandle.DestroyAndFree(ref this.handle);
		}

		internal int Length
		{
			get
			{
				return this.handle.Length;
			}
		}

		internal IntPtr this[int i]
		{
			get
			{
				return this.handle[i];
			}
		}

		private RuntimeGPtrArrayHandle handle;
	}
}
