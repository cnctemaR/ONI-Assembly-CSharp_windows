using System;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements
{
	[StructLayout(LayoutKind.Explicit, Size = 32)]
	internal readonly struct VisualNodeClassDataAlloc
	{
		public bool IsCreated
		{
			get
			{
				return (this.m_Reserved & int.MinValue) != 0;
			}
		}

		public int Count
		{
			get
			{
				return this.m_Size;
			}
		}

		public unsafe int this[int index]
		{
			get
			{
				bool flag = (ulong)index >= (ulong)((long)this.m_Size);
				if (flag)
				{
					throw new IndexOutOfRangeException("index");
				}
				return this.GetUnsafePtr()[index];
			}
		}

		public unsafe int* GetUnsafePtr()
		{
			return (int*)this.m_Ptr.ToPointer();
		}

		private const int k_VisualNodeClassDataIsAllocBit = -2147483648;

		[FieldOffset(0)]
		private readonly IntPtr m_Ptr;

		[FieldOffset(8)]
		private readonly int m_Size;

		[FieldOffset(12)]
		private readonly int m_Capacity;

		[FieldOffset(16)]
		private readonly int m_Reserved;
	}
}
