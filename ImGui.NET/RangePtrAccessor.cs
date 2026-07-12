using System;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct RangePtrAccessor<T> where T : struct
	{
		public RangePtrAccessor(IntPtr data, int count)
		{
			this = new RangePtrAccessor<T>(data.ToPointer(), count);
		}

		public unsafe RangePtrAccessor(void* data, int count)
		{
			this.Data = data;
			this.Count = count;
		}

		public unsafe T this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new IndexOutOfRangeException();
				}
				return Unsafe.Read<T>((void*)((byte*)this.Data + sizeof(void*) * index));
			}
		}

		public unsafe readonly void* Data;

		public readonly int Count;
	}
}
