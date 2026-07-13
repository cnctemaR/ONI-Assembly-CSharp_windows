using System;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements
{
	[StructLayout(LayoutKind.Explicit, Size = 32)]
	internal readonly struct VisualNodeChildrenFixed
	{
		public unsafe int Count
		{
			get
			{
				fixed (VisualNodeHandle* ptr = &this.__Child0)
				{
					VisualNodeHandle* ptr2 = ptr;
					int i;
					for (i = 0; i < 4; i++)
					{
						bool flag = ptr2[i].Id == 0;
						if (flag)
						{
							return i;
						}
					}
					return i;
				}
			}
		}

		public unsafe VisualNodeHandle this[int index]
		{
			get
			{
				bool flag = index >= 4;
				if (flag)
				{
					throw new IndexOutOfRangeException("index");
				}
				fixed (VisualNodeHandle* ptr = &this.__Child0)
				{
					VisualNodeHandle* ptr2 = ptr;
					return ptr2[index];
				}
			}
		}

		public unsafe VisualNodeHandle* GetUnsafePtr()
		{
			fixed (VisualNodeHandle* ptr = &this.__Child0)
			{
				return ptr;
			}
		}

		private const int k_VisualNodeChildrenFixedCapacity = 4;

		[FieldOffset(0)]
		private readonly VisualNodeHandle __Child0;

		[FieldOffset(8)]
		private readonly VisualNodeHandle __Child1;

		[FieldOffset(16)]
		private readonly VisualNodeHandle __Child2;

		[FieldOffset(24)]
		private readonly VisualNodeHandle __Child3;
	}
}
