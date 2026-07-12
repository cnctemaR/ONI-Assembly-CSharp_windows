using System;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct ImPtrVector<T>
	{
		public ImPtrVector(ImVector vector, int stride)
		{
			this = new ImPtrVector<T>(vector.Size, vector.Capacity, vector.Data, stride);
		}

		public ImPtrVector(int size, int capacity, IntPtr data, int stride)
		{
			this.Size = size;
			this.Capacity = capacity;
			this.Data = data;
			this._stride = stride;
		}

		public unsafe T this[int index]
		{
			get
			{
				byte* ptr = (byte*)(void*)this.Data + index * this._stride;
				return Unsafe.Read<T>((void*)(&ptr));
			}
		}

		public readonly int Size;

		public readonly int Capacity;

		public readonly IntPtr Data;

		private readonly int _stride;
	}
}
