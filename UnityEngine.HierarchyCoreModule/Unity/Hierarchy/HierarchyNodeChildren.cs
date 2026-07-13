using System;
using System.Runtime.CompilerServices;

namespace Unity.Hierarchy
{
	public readonly struct HierarchyNodeChildren
	{
		internal unsafe HierarchyNodeChildren(Hierarchy hierarchy, IntPtr nodeChildrenPtr)
		{
			bool flag = hierarchy == null;
			if (flag)
			{
				throw new ArgumentNullException("hierarchy");
			}
			bool flag2 = nodeChildrenPtr == IntPtr.Zero;
			if (flag2)
			{
				throw new ArgumentNullException("nodeChildrenPtr");
			}
			this.m_Hierarchy = hierarchy;
			this.m_Version = hierarchy.Version;
			ref HierarchyNodeChildrenAlloc ptr = ref *(HierarchyNodeChildrenAlloc*)(void*)nodeChildrenPtr;
			bool flag3 = (ptr.ControlBit & int.MinValue) == int.MinValue;
			if (flag3)
			{
				this.m_Ptr = ptr.Ptr;
				this.m_Count = ptr.Size;
			}
			else
			{
				this.m_Ptr = (HierarchyNode*)(void*)nodeChildrenPtr;
				this.m_Count = 0;
				for (int i = 0; i < 4; i++)
				{
					bool flag4 = (in this.m_Ptr[i]) != HierarchyNode.Null;
					if (!flag4)
					{
						break;
					}
					this.m_Count++;
				}
			}
		}

		public int Count
		{
			get
			{
				this.ThrowIfVersionChanged();
				return this.m_Count;
			}
		}

		public ref HierarchyNode this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				bool flag = index < 0 || index >= this.m_Count;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				this.ThrowIfVersionChanged();
				return this.m_Ptr + index;
			}
		}

		public HierarchyNodeChildren.Enumerator GetEnumerator()
		{
			return new HierarchyNodeChildren.Enumerator(in this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ThrowIfVersionChanged()
		{
			bool flag = this.m_Version != this.m_Hierarchy.Version;
			if (flag)
			{
				throw new InvalidOperationException("Hierarchy was modified.");
			}
		}

		private const int k_HierarchyNodeChildrenIsAllocBit = -2147483648;

		private readonly Hierarchy m_Hierarchy;

		private unsafe readonly HierarchyNode* m_Ptr;

		private readonly int m_Version;

		private readonly int m_Count;

		public struct Enumerator
		{
			internal Enumerator(in HierarchyNodeChildren enumerable)
			{
				this.m_Enumerable = enumerable;
				this.m_Index = -1;
			}

			public readonly ref HierarchyNode Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					this.m_Enumerable.ThrowIfVersionChanged();
					return this.m_Enumerable.m_Ptr + this.m_Index;
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				int num = this.m_Index + 1;
				this.m_Index = num;
				return num < this.m_Enumerable.m_Count;
			}

			private readonly HierarchyNodeChildren m_Enumerable;

			private int m_Index;
		}
	}
}
