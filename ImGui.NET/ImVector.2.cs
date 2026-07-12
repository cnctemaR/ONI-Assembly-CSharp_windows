using System;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct ImVector<T>
	{
		public ImVector(ImVector vector)
		{
			this.Size = vector.Size;
			this.Capacity = vector.Capacity;
			this.Data = vector.Data;
		}

		public ImVector(int size, int capacity, IntPtr data)
		{
			this.Size = size;
			this.Capacity = capacity;
			this.Data = data;
		}

		public unsafe ref T this[int index]
		{
			get
			{
				return Unsafe.AsRef<T>((void*)((byte*)(void*)this.Data + index * Unsafe.SizeOf<T>()));
			}
		}

		public readonly int Size;

		public readonly int Capacity;

		public readonly IntPtr Data;
	}
}
