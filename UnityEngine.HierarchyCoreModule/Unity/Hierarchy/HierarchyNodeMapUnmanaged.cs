using System;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace Unity.Hierarchy
{
	public struct HierarchyNodeMapUnmanaged<[global::System.Runtime.CompilerServices.IsUnmanaged] T> : IDisposable where T : struct, ValueType
	{
		public bool IsCreated
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Values.IsCreated;
			}
		}

		public int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Values.Capacity;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Values.Capacity = value;
			}
		}

		public int Count
		{
			get
			{
				return this.m_Values.Count;
			}
		}

		public T this[in HierarchyNode node]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Values[ref node];
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Values[ref node] = value;
			}
		}

		public HierarchyNodeMapUnmanaged(Allocator allocator)
		{
			this.m_Values = new NativeSparseArray<HierarchyNode, T>(new NativeSparseArray<HierarchyNode, T>.KeyIndex(HierarchyNodeMapUnmanaged<T>.KeyIndex), new NativeSparseArray<HierarchyNode, T>.KeyEqual(HierarchyNodeMapUnmanaged<T>.KeyEqual), allocator);
		}

		public HierarchyNodeMapUnmanaged(in T initValue, Allocator allocator)
		{
			this.m_Values = new NativeSparseArray<HierarchyNode, T>(in initValue, new NativeSparseArray<HierarchyNode, T>.KeyIndex(HierarchyNodeMapUnmanaged<T>.KeyIndex), new NativeSparseArray<HierarchyNode, T>.KeyEqual(HierarchyNodeMapUnmanaged<T>.KeyEqual), allocator);
		}

		public void Dispose()
		{
			this.m_Values.Dispose();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reserve(int capacity)
		{
			this.m_Values.Reserve(capacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ContainsKey(in HierarchyNode node)
		{
			return this.m_Values.ContainsKey(in node);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(in HierarchyNode node, in T value)
		{
			this.m_Values.Add(in node, in value, NativeSparseArrayResizePolicy.ExactSize);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddNoResize(in HierarchyNode node, in T value)
		{
			this.m_Values.AddNoResize(in node, in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryAdd(in HierarchyNode node, in T value)
		{
			return this.m_Values.TryAdd(in node, in value, NativeSparseArrayResizePolicy.ExactSize);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryAddNoResize(in HierarchyNode node, in T value)
		{
			return this.m_Values.TryAddNoResize(in node, in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetValue(in HierarchyNode node, out T value)
		{
			return this.m_Values.TryGetValue(in node, out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove(in HierarchyNode node)
		{
			return this.m_Values.Remove(in node);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			this.m_Values.Clear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int KeyIndex(in HierarchyNode node)
		{
			return node.Id - 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool KeyEqual(in HierarchyNode lhs, in HierarchyNode rhs)
		{
			return lhs.Version == rhs.Version;
		}

		private NativeSparseArray<HierarchyNode, T> m_Values;
	}
}
