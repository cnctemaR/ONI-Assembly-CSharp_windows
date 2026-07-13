using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	internal sealed class NativeRingQueueDebugView<[global::System.Runtime.CompilerServices.IsUnmanaged] T> where T : struct, ValueType
	{
		public NativeRingQueueDebugView(NativeRingQueue<T> data)
		{
			this.Data = data.m_RingQueue;
		}

		public unsafe T[] Items
		{
			get
			{
				T[] array = new T[this.Data->Length];
				int read = this.Data->m_Read;
				int capacity = this.Data->m_Capacity;
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this.Data->Ptr[(IntPtr)((read + i) % capacity) * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
				}
				return array;
			}
		}

		private unsafe UnsafeRingQueue<T>* Data;
	}
}
