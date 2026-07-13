using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	internal sealed class NativeListDebugView<[global::System.Runtime.CompilerServices.IsUnmanaged] T> where T : struct, ValueType
	{
		public NativeListDebugView(NativeList<T> array)
		{
			this.Data = array.m_ListData;
		}

		public unsafe T[] Items
		{
			get
			{
				if (this.Data == null)
				{
					return null;
				}
				int length = this.Data->Length;
				T[] array = new T[length];
				fixed (T* ptr = &array[0])
				{
					UnsafeUtility.MemCpy((void*)ptr, (void*)this.Data->Ptr, (long)(length * UnsafeUtility.SizeOf<T>()));
				}
				return array;
			}
		}

		private unsafe UnsafeList<T>* Data;
	}
}
