using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Unity.Hierarchy
{
	public readonly struct HierarchyNodeTypeHandlerBaseEnumerable
	{
		internal HierarchyNodeTypeHandlerBaseEnumerable(Hierarchy hierarchy)
		{
			this.m_Hierarchy = hierarchy;
		}

		public HierarchyNodeTypeHandlerBaseEnumerable.Enumerator GetEnumerator()
		{
			return new HierarchyNodeTypeHandlerBaseEnumerable.Enumerator(this.m_Hierarchy);
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

			public HierarchyNodeTypeHandlerBase Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return HierarchyNodeTypeHandlerBase.FromIntPtr(this.m_Handlers[this.m_Index]);
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				int num = this.m_Index + 1;
				this.m_Index = num;
				return num < this.m_Count;
			}

			private readonly IntPtr[] m_Handlers;

			private readonly int m_Count;

			private int m_Index;
		}
	}
}
