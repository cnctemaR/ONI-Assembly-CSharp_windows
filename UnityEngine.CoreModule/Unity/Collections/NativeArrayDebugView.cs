using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	internal sealed class NativeArrayDebugView<T> where T : struct
	{
		public NativeArrayDebugView(NativeArray<T> array)
		{
			this.m_Array = array;
		}

		public unsafe T[] Items
		{
			get
			{
				bool flag = !this.m_Array.IsCreated;
				T[] array;
				if (flag)
				{
					array = null;
				}
				else
				{
					int length = this.m_Array.m_Length;
					T[] array2 = new T[length];
					GCHandle gchandle = GCHandle.Alloc(array2, GCHandleType.Pinned);
					IntPtr intPtr = gchandle.AddrOfPinnedObject();
					UnsafeUtility.MemCpy((void*)intPtr, this.m_Array.m_Buffer, (long)(length * UnsafeUtility.SizeOf<T>()));
					gchandle.Free();
					array = array2;
				}
				return array;
			}
		}

		private NativeArray<T> m_Array;
	}
}
