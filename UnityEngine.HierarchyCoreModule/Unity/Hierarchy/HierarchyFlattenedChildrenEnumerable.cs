using System;
using System.Runtime.CompilerServices;

namespace Unity.Hierarchy
{
	public readonly struct HierarchyFlattenedChildrenEnumerable
	{
		internal unsafe HierarchyFlattenedChildrenEnumerable(HierarchyFlattened hierarchyFlattened, in HierarchyNode node)
		{
			bool flag = hierarchyFlattened == null || !hierarchyFlattened.IsCreated;
			if (flag)
			{
				throw new ArgumentNullException("hierarchyFlattened");
			}
			bool flag2 = (in node) == HierarchyNode.Null;
			if (flag2)
			{
				throw new ArgumentNullException("node");
			}
			bool flag3 = !hierarchyFlattened.Contains(in node);
			if (flag3)
			{
				throw new InvalidOperationException(string.Format("{0} not found", node));
			}
			this.m_HierarchyFlattened = hierarchyFlattened;
			this.m_ParentIndex = this.m_HierarchyFlattened.IndexOf(in node);
			this.m_ParentNode = *this.m_HierarchyFlattened[this.m_ParentIndex];
		}

		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_ParentNode.ChildrenCount;
			}
		}

		public HierarchyFlattenedChildrenEnumerable.Enumerator GetEnumerator()
		{
			return new HierarchyFlattenedChildrenEnumerable.Enumerator(this);
		}

		private readonly HierarchyFlattened m_HierarchyFlattened;

		private readonly HierarchyFlattenedNode m_ParentNode;

		private readonly int m_ParentIndex;

		public struct Enumerator
		{
			internal Enumerator(HierarchyFlattenedChildrenEnumerable enumerable)
			{
				this.m_Enumerable = enumerable;
				this.m_End = this.m_Enumerable.m_ParentIndex + this.m_Enumerable.m_ParentNode.NextSiblingOffset;
				this.m_Depth = this.m_Enumerable.m_ParentNode.Depth + 1;
				this.m_Version = this.m_Enumerable.m_HierarchyFlattened.Version;
				this.m_Current = this.m_Enumerable.m_ParentIndex;
			}

			public readonly ref HierarchyFlattenedNode Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					this.ThrowIfVersionChanged();
					return this.m_Enumerable.m_HierarchyFlattened[this.m_Current];
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				this.ThrowIfVersionChanged();
				bool flag = this.m_Current == this.m_Enumerable.m_ParentIndex;
				if (flag)
				{
					this.m_Current++;
				}
				else
				{
					this.m_Current += this.m_Enumerable.m_HierarchyFlattened[this.m_Current].NextSiblingOffset;
				}
				return this.m_Current < this.m_End;
			}

			public void Reset()
			{
				this.ThrowIfVersionChanged();
				this.m_Current = this.m_Enumerable.m_ParentIndex;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private void ThrowIfVersionChanged()
			{
				bool flag = this.m_Version != this.m_Enumerable.m_HierarchyFlattened.Version;
				if (flag)
				{
					throw new InvalidOperationException("HierarchyFlattened was modified during enumeration.");
				}
			}

			private readonly HierarchyFlattenedChildrenEnumerable m_Enumerable;

			private readonly int m_End;

			private readonly int m_Depth;

			private readonly int m_Version;

			private int m_Current;
		}
	}
}
