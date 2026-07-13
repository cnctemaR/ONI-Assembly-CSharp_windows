using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Hierarchy
{
	[StructLayout(LayoutKind.Explicit, Size = 32)]
	internal struct HierarchyNodeChildrenFixed
	{
		public unsafe HierarchyNode* Ptr
		{
			get
			{
				return (HierarchyNode*)UnsafeUtility.AddressOf<HierarchyNode>(ref this.m_Node1);
			}
		}

		public const int Capacity = 4;

		[FieldOffset(0)]
		private HierarchyNode m_Node1;

		[FieldOffset(8)]
		private HierarchyNode m_Node2;

		[FieldOffset(16)]
		private HierarchyNode m_Node3;

		[FieldOffset(24)]
		private HierarchyNode m_Node4;
	}
}
