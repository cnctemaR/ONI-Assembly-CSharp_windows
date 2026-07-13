using System;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements
{
	[StructLayout(LayoutKind.Explicit, Size = 32)]
	internal readonly struct VisualNodeClassDataFixed
	{
		public unsafe int Count
		{
			get
			{
				fixed (int* ptr = &this.__Child0)
				{
					int* ptr2 = ptr;
					int i;
					for (i = 0; i < 8; i++)
					{
						bool flag = ptr2[i] == 0;
						if (flag)
						{
							return i;
						}
					}
					return i;
				}
			}
		}

		public unsafe int this[int index]
		{
			get
			{
				bool flag = index >= 8;
				if (flag)
				{
					throw new IndexOutOfRangeException("index");
				}
				fixed (int* ptr = &this.__Child0)
				{
					int* ptr2 = ptr;
					return ptr2[index];
				}
			}
		}

		public unsafe int* GetUnsafePtr()
		{
			fixed (int* ptr = &this.__Child0)
			{
				return ptr;
			}
		}

		private const int k_VisualNodeClassDataFixedCapacity = 8;

		[FieldOffset(0)]
		private readonly int __Child0;

		[FieldOffset(4)]
		private readonly int __Child1;

		[FieldOffset(8)]
		private readonly int __Child2;

		[FieldOffset(12)]
		private readonly int __Child3;

		[FieldOffset(16)]
		private readonly int __Child4;

		[FieldOffset(20)]
		private readonly int __Child5;

		[FieldOffset(24)]
		private readonly int __Child6;

		[FieldOffset(28)]
		private readonly int __Child7;
	}
}
