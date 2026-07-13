using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections.LowLevel.Unsafe
{
	internal sealed class UnsafeRingQueueDebugView<[global::System.Runtime.CompilerServices.IsUnmanaged] T> where T : struct, ValueType
	{
		public UnsafeRingQueueDebugView(UnsafeRingQueue<T> data)
		{
			this.Data = data;
		}

		public unsafe T[] Items
		{
			get
			{
				T[] array = new T[this.Data.Length];
				int read = this.Data.Control.Read;
				int capacity = this.Data.Control.Capacity;
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this.Data.Ptr[(IntPtr)((read + i) % capacity) * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
				}
				return array;
			}
		}

		private UnsafeRingQueue<T> Data;
	}
}
