using System;
using System.Runtime.CompilerServices;

namespace Mono
{
	internal struct RuntimeGPtrArrayHandle
	{
		internal unsafe RuntimeGPtrArrayHandle(RuntimeStructs.GPtrArray* value)
		{
			this.value = value;
		}

		internal unsafe RuntimeGPtrArrayHandle(IntPtr ptr)
		{
			this.value = (RuntimeStructs.GPtrArray*)(void*)ptr;
		}

		internal unsafe int Length
		{
			get
			{
				return this.value->len;
			}
		}

		internal IntPtr this[int i]
		{
			get
			{
				return this.Lookup(i);
			}
		}

		internal unsafe IntPtr Lookup(int i)
		{
			if (i >= 0 && i < this.Length)
			{
				return this.value->data[i];
			}
			throw new IndexOutOfRangeException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void GPtrArrayFree(RuntimeStructs.GPtrArray* value);

		internal static void DestroyAndFree(ref RuntimeGPtrArrayHandle h)
		{
			RuntimeGPtrArrayHandle.GPtrArrayFree(h.value);
			h.value = null;
		}

		private unsafe RuntimeStructs.GPtrArray* value;
	}
}
