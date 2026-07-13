using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Unity.Hierarchy
{
	internal readonly struct HierarchyNodeTypeHandlerEnumerable
	{
		internal HierarchyNodeTypeHandlerEnumerable(Hierarchy hierarchy)
		{
			this.m_Hierarchy = hierarchy;
		}

		public HierarchyNodeTypeHandlerEnumerable.Enumerator GetEnumerator()
		{
			return new HierarchyNodeTypeHandlerEnumerable.Enumerator(this.m_Hierarchy);
		}

		private readonly Hierarchy m_Hierarchy;

		public struct Enumerator : IDisposable
		{
			internal Enumerator(Hierarchy hierarchy)
			{
				int nodeTypeHandlersBaseCount = hierarchy.GetNodeTypeHandlersBaseCount();
				this.m_Handlers = ArrayPool<IntPtr>.Shared.Rent(nodeTypeHandlersBaseCount);
				this.m_Count = hierarchy.GetNodeTypeHandlersBaseSpan(this.m_Handlers.AsSpan<IntPtr>().Slice(0, nodeTypeHandlersBaseCount));
				this.m_Index = -1;
			}

			public void Dispose()
			{
				ArrayPool<IntPtr>.Shared.Return(this.m_Handlers, false);
			}

			public HierarchyNodeTypeHandler Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return HierarchyNodeTypeHandlerBase.FromIntPtr(this.m_Handlers[this.m_Index]) as HierarchyNodeTypeHandler;
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				bool flag;
				do
				{
					int num = this.m_Index + 1;
					this.m_Index = num;
					if (num >= this.m_Count)
					{
						goto Block_2;
					}
					flag = HierarchyNodeTypeHandlerBase.FromIntPtr(this.m_Handlers[this.m_Index]) is HierarchyNodeTypeHandler;
				}
				while (!flag);
				return true;
				Block_2:
				return false;
			}

			private readonly IntPtr[] m_Handlers;

			private readonly int m_Count;

			private int m_Index;
		}
	}
}
