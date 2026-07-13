using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Hierarchy
{
	[NativeHeader("Modules/HierarchyCore/HierarchyFlattenedBindings.h")]
	[NativeHeader("Modules/HierarchyCore/Public/HierarchyFlattened.h")]
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class HierarchyFlattened : IDisposable
	{
		public bool IsCreated
		{
			get
			{
				return this.m_Ptr != IntPtr.Zero;
			}
		}

		public int Count
		{
			get
			{
				return this.m_FlattenedNodes.Count;
			}
		}

		public bool Updating
		{
			[NativeMethod("Updating", IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = HierarchyFlattened.BindingsMarshaller.ConvertToUnmanaged(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HierarchyFlattened.get_Updating_Injected(intPtr);
			}
		}

		public bool UpdateNeeded
		{
			[NativeMethod("UpdateNeeded", IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = HierarchyFlattened.BindingsMarshaller.ConvertToUnmanaged(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HierarchyFlattened.get_UpdateNeeded_Injected(intPtr);
			}
		}

		internal ReadOnlyNativeVector<HierarchyFlattenedNode> FlattenedNodes
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_FlattenedNodes;
			}
		}

		internal int Version
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Version;
			}
		}

		public HierarchyFlattened(Hierarchy hierarchy)
		{
			IntPtr intPtr;
			int num;
			int num2;
			this.m_Ptr = HierarchyFlattened.Create(GCHandle.ToIntPtr(GCHandle.Alloc(this)), hierarchy, out intPtr, out num, out num2);
			this.m_Hierarchy = hierarchy;
			this.m_FlattenedNodes = new ReadOnlyNativeVector<HierarchyFlattenedNode>(intPtr, num);
			this.m_Version = num2;
			this.m_IsOwner = true;
		}

		private HierarchyFlattened(IntPtr nativePtr, Hierarchy hierarchy, IntPtr flattenedNodesPtr, int flattenedNodesCount, int version)
		{
			this.m_Ptr = nativePtr;
			this.m_Hierarchy = hierarchy;
			this.m_FlattenedNodes = new ReadOnlyNativeVector<HierarchyFlattenedNode>(flattenedNodesPtr, flattenedNodesCount);
			this.m_Version = version;
			this.m_IsOwner = false;
		}

		~HierarchyFlattened()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			bool flag = this.m_Ptr != IntPtr.Zero;
			if (flag)
			{
				bool isOwner = this.m_IsOwner;
				if (isOwner)
				{
					HierarchyFlattened.Destroy(this.m_Ptr);
				}
				this.m_Ptr = IntPtr.Zero;
			}
			this.m_FlattenedNodes = default(ReadOnlyNativeVector<HierarchyFlattenedNode>);
		}

		public ref HierarchyFlattenedNode this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_FlattenedNodes[index];
			}
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int IndexOf(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyFlattened.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyFlattened.IndexOf_Injected(intPtr, in node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool Contains(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyFlattened.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyFlattened.Contains_Injected(intPtr, in node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public HierarchyNode GetParent(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyFlattened.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyNode hierarchyNode;
			HierarchyFlattened.GetParent_Injected(intPtr, in node, out hierarchyNode);
			return hierarchyNode;
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public HierarchyNode GetNextSibling(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyFlattened.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyNode hierarchyNode;
			HierarchyFlattened.GetNextSibling_Injected(intPtr, in node, out hierarchyNode);
			return hierarchyNode;
		}

		public HierarchyFlattenedChildrenEnumerable EnumerateChildren(in HierarchyNode node)
		{
			return new HierarchyFlattenedChildrenEnumerable(this, in node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int GetChildrenCount(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyFlattened.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyFlattened.GetChildrenCount_Injected(intPtr, in node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int GetChildrenCountRecursive(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyFlattened.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyFlattened.GetChildrenCountRecursive_Injected(intPtr, in node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public HierarchyNode GetChild(in HierarchyNode node, int index)
		{
			IntPtr intPtr = HierarchyFlattened.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyNode hierarchyNode;
			HierarchyFlattened.GetChild_Injected(intPtr, in node, index, out hierarchyNode);
			return hierarchyNode;
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int GetChildIndex(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyFlattened.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyFlattened.GetChildIndex_Injected(intPtr, in node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int GetDepth(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyFlattened.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyFlattened.GetDepth_Injected(intPtr, in node);
		}

		[NativeMethod(IsThreadSafe = true)]
		public void Update()
		{
			IntPtr intPtr = HierarchyFlattened.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyFlattened.Update_Injected(intPtr);
		}

		[NativeMethod(IsThreadSafe = true)]
		public bool UpdateIncremental()
		{
			IntPtr intPtr = HierarchyFlattened.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyFlattened.UpdateIncremental_Injected(intPtr);
		}

		[NativeMethod(IsThreadSafe = true)]
		public bool UpdateIncrementalTimed(double milliseconds)
		{
			IntPtr intPtr = HierarchyFlattened.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyFlattened.UpdateIncrementalTimed_Injected(intPtr, milliseconds);
		}

		public HierarchyFlattened.Enumerator GetEnumerator()
		{
			return new HierarchyFlattened.Enumerator(this);
		}

		public ReadOnlySpan<HierarchyFlattenedNode> AsReadOnlySpan()
		{
			return this.m_FlattenedNodes.AsReadOnlySpan();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static HierarchyFlattened FromIntPtr(IntPtr handlePtr)
		{
			return (handlePtr != IntPtr.Zero) ? ((HierarchyFlattened)GCHandle.FromIntPtr(handlePtr).Target) : null;
		}

		[FreeFunction("HierarchyFlattenedBindings::Create", IsThreadSafe = true)]
		private static IntPtr Create(IntPtr handlePtr, Hierarchy hierarchy, out IntPtr nodesPtr, out int nodesCount, out int version)
		{
			return HierarchyFlattened.Create_Injected(handlePtr, (hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy), out nodesPtr, out nodesCount, out version);
		}

		[FreeFunction("HierarchyFlattenedBindings::Destroy", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy(IntPtr nativePtr);

		[RequiredByNativeCode]
		private static IntPtr CreateHierarchyFlattened(IntPtr nativePtr, IntPtr hierarchyPtr, IntPtr flattenedNodesPtr, int flattenedNodesCount, int version)
		{
			return GCHandle.ToIntPtr(GCHandle.Alloc(new HierarchyFlattened(nativePtr, Hierarchy.FromIntPtr(hierarchyPtr), flattenedNodesPtr, flattenedNodesCount, version)));
		}

		[RequiredByNativeCode]
		private static void UpdateHierarchyFlattened(IntPtr handlePtr, IntPtr flattenedNodesPtr, int flattenedNodesCount, int version)
		{
			HierarchyFlattened hierarchyFlattened = HierarchyFlattened.FromIntPtr(handlePtr);
			hierarchyFlattened.m_FlattenedNodes = new ReadOnlyNativeVector<HierarchyFlattenedNode>(flattenedNodesPtr, flattenedNodesCount);
			hierarchyFlattened.m_Version = version;
		}

		[Obsolete("The Hierarchy property will be removed in the future, remove its usage from your code.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Hierarchy Hierarchy
		{
			get
			{
				return this.m_Hierarchy;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_Updating_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_UpdateNeeded_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int IndexOf_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Contains_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetParent_Injected(IntPtr _unity_self, in HierarchyNode node, out HierarchyNode ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetNextSibling_Injected(IntPtr _unity_self, in HierarchyNode node, out HierarchyNode ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetChildrenCount_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetChildrenCountRecursive_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetChild_Injected(IntPtr _unity_self, in HierarchyNode node, int index, out HierarchyNode ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetChildIndex_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetDepth_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Update_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool UpdateIncremental_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool UpdateIncrementalTimed_Injected(IntPtr _unity_self, double milliseconds);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create_Injected(IntPtr handlePtr, IntPtr hierarchy, out IntPtr nodesPtr, out int nodesCount, out int version);

		private IntPtr m_Ptr;

		internal readonly Hierarchy m_Hierarchy;

		private ReadOnlyNativeVector<HierarchyFlattenedNode> m_FlattenedNodes;

		private int m_Version;

		private readonly bool m_IsOwner;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToUnmanaged(HierarchyFlattened hierarchyFlattened)
			{
				return hierarchyFlattened.m_Ptr;
			}
		}

		public struct Enumerator
		{
			internal Enumerator(HierarchyFlattened hierarchyFlattened)
			{
				this.m_HierarchyFlattened = hierarchyFlattened;
				this.m_FlattenedNodes = hierarchyFlattened.m_FlattenedNodes;
				this.m_Version = hierarchyFlattened.m_Version;
				this.m_Index = -1;
			}

			public readonly ref HierarchyFlattenedNode Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					bool flag = this.m_Version != this.m_HierarchyFlattened.m_Version;
					if (flag)
					{
						throw new InvalidOperationException("HierarchyFlattened was modified.");
					}
					return this.m_FlattenedNodes[this.m_Index];
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				int num = this.m_Index + 1;
				this.m_Index = num;
				return num < this.m_FlattenedNodes.Count;
			}

			private readonly HierarchyFlattened m_HierarchyFlattened;

			private readonly ReadOnlyNativeVector<HierarchyFlattenedNode> m_FlattenedNodes;

			private readonly int m_Version;

			private int m_Index;
		}
	}
}
