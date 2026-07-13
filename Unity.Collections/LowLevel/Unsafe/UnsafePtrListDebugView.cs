using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	internal sealed class UnsafePtrListDebugView
	{
		public UnsafePtrListDebugView(UnsafePtrList data)
		{
			this.Data = data;
		}

		public unsafe IntPtr[] Items
		{
			get
			{
				IntPtr[] array = new IntPtr[this.Data.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (IntPtr)(*(IntPtr*)(this.Data.Ptr + (IntPtr)i * (IntPtr)sizeof(void*) / (IntPtr)sizeof(void*)));
				}
				return array;
			}
		}

		private UnsafePtrList Data;
	}
}
