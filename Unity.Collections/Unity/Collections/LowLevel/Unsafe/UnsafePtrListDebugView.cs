using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections.LowLevel.Unsafe
{
	internal sealed class UnsafePtrListDebugView<[global::System.Runtime.CompilerServices.IsUnmanaged] T> where T : struct, ValueType
	{
		public UnsafePtrListDebugView(UnsafePtrList<T> data)
		{
			this.Data = data;
		}

		public unsafe T*[] Items
		{
			get
			{
				T*[] array = new T*[this.Data.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = *(IntPtr*)(this.Data.Ptr + (IntPtr)i * (IntPtr)sizeof(T*) / (IntPtr)sizeof(T*));
				}
				return array;
			}
		}

		private UnsafePtrList<T> Data;
	}
}
