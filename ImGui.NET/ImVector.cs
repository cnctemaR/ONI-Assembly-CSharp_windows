using System;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct ImVector
	{
		public unsafe ref T Ref<T>(int index)
		{
			return Unsafe.AsRef<T>((void*)((byte*)(void*)this.Data + index * Unsafe.SizeOf<T>()));
		}

		public unsafe IntPtr Address<T>(int index)
		{
			return (IntPtr)((void*)((byte*)(void*)this.Data + index * Unsafe.SizeOf<T>()));
		}

		public readonly int Size;

		public readonly int Capacity;

		public readonly IntPtr Data;
	}
}
