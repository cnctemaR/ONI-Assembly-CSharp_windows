using System;
using System.Runtime.InteropServices;

namespace Unity.Hierarchy
{
	[StructLayout(LayoutKind.Explicit, Size = 32)]
	internal struct HierarchyNodeChildrenAlloc
	{
		[FieldOffset(0)]
		public unsafe HierarchyNode* Ptr;

		[FieldOffset(8)]
		public int Size;

		[FieldOffset(12)]
		public int Capacity;

		[FieldOffset(16)]
		public int ControlBit;

		[FieldOffset(20)]
		public int NullCount;

		[FieldOffset(24)]
		public int Reserved0;

		[FieldOffset(28)]
		public int Reserved1;
	}
}
