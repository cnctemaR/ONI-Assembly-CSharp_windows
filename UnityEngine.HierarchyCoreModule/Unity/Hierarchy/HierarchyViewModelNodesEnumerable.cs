using System;
using System.Runtime.CompilerServices;

namespace Unity.Hierarchy
{
	public readonly struct HierarchyViewModelNodesEnumerable
	{
		internal HierarchyViewModelNodesEnumerable(HierarchyViewModel viewModel, HierarchyNodeFlags flags, HierarchyViewModelNodesEnumerable.Predicate predicate)
		{
			if (viewModel == null)
			{
				throw new ArgumentNullException("viewModel");
			}
			this.m_HierarchyViewModel = viewModel;
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			this.m_Predicate = predicate;
			this.m_Flags = flags;
		}

		public HierarchyViewModelNodesEnumerable.Enumerator GetEnumerator()
		{
			return new HierarchyViewModelNodesEnumerable.Enumerator(this);
		}

		private readonly HierarchyViewModel m_HierarchyViewModel;

		private readonly HierarchyViewModelNodesEnumerable.Predicate m_Predicate;

		private readonly HierarchyNodeFlags m_Flags;

		internal delegate bool Predicate(in HierarchyNode node, HierarchyNodeFlags flags);

		public struct Enumerator
		{
			internal Enumerator(HierarchyViewModelNodesEnumerable enumerable)
			{
				this.m_HierarchyViewModel = enumerable.m_HierarchyViewModel;
				this.m_Predicate = enumerable.m_Predicate;
				this.m_Flags = enumerable.m_Flags;
				this.m_FlattenedNodes = this.m_HierarchyViewModel.FlattenedNodes;
				this.m_Version = this.m_HierarchyViewModel.Version;
				this.m_Index = 0;
			}

			public readonly ref HierarchyNode Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					this.ThrowIfVersionChanged();
					return HierarchyFlattenedNode.GetNodeByRef(this.m_FlattenedNodes[this.m_Index]);
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				this.ThrowIfVersionChanged();
				for (;;)
				{
					int num = this.m_Index + 1;
					this.m_Index = num;
					bool flag = num >= this.m_FlattenedNodes.Count;
					if (flag)
					{
						break;
					}
					bool flag2 = this.m_Predicate(HierarchyFlattenedNode.GetNodeByRef(this.m_FlattenedNodes[this.m_Index]), this.m_Flags);
					if (flag2)
					{
						goto Block_2;
					}
				}
				return false;
				Block_2:
				return true;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private void ThrowIfVersionChanged()
			{
				bool flag = this.m_Version != this.m_HierarchyViewModel.Version;
				if (flag)
				{
					throw new InvalidOperationException("HierarchyViewModel was modified.");
				}
			}

			private readonly HierarchyViewModel m_HierarchyViewModel;

			private readonly HierarchyViewModelNodesEnumerable.Predicate m_Predicate;

			private readonly HierarchyNodeFlags m_Flags;

			private readonly ReadOnlyNativeVector<HierarchyFlattenedNode> m_FlattenedNodes;

			private readonly int m_Version;

			private int m_Index;
		}
	}
}
