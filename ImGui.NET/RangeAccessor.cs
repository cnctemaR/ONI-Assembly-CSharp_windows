using System;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct RangeAccessor<T> where T : struct
	{
		public RangeAccessor(IntPtr data, int count)
		{
			this = new RangeAccessor<T>(data.ToPointer(), count);
		}

		public unsafe RangeAccessor(void* data, int count)
		{
			this.Data = data;
			this.Count = count;
		}

		public unsafe ref T this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new IndexOutOfRangeException();
				}
				return Unsafe.AsRef<T>((void*)((byte*)this.Data + RangeAccessor<T>.s_sizeOfT * index));
			}
		}

		private static readonly int s_sizeOfT = Unsafe.SizeOf<T>();

		public unsafe readonly void* Data;

		public readonly int Count;
	}
}
