using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Hierarchy
{
	[NativeHeader("Modules/HierarchyCore/Public/HierarchyCommandList.h")]
	[NativeHeader("Modules/HierarchyCore/HierarchyCommandListBindings.h")]
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class HierarchyCommandList : IDisposable
	{
		public bool IsCreated
		{
			get
			{
				return this.m_Ptr != IntPtr.Zero;
			}
		}

		public int Size
		{
			[NativeMethod("Size", IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HierarchyCommandList.get_Size_Injected(intPtr);
			}
		}

		public int Capacity
		{
			[NativeMethod("Capacity", IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HierarchyCommandList.get_Capacity_Injected(intPtr);
			}
		}

		public bool IsEmpty
		{
			[NativeMethod("IsEmpty", IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HierarchyCommandList.get_IsEmpty_Injected(intPtr);
			}
		}

		public bool IsExecuting
		{
			[NativeMethod("IsExecuting", IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HierarchyCommandList.get_IsExecuting_Injected(intPtr);
			}
		}

		public unsafe HierarchyCommandList(Hierarchy hierarchy, int initialCapacity = 65536)
			: this(hierarchy, *HierarchyNodeType.Null, initialCapacity)
		{
		}

		internal HierarchyCommandList(Hierarchy hierarchy, HierarchyNodeType nodeType, int initialCapacity = 65536)
		{
			this.m_Ptr = HierarchyCommandList.Create(GCHandle.ToIntPtr(GCHandle.Alloc(this)), hierarchy, nodeType, initialCapacity);
			this.m_IsOwner = true;
		}

		private HierarchyCommandList(IntPtr nativePtr)
		{
			this.m_Ptr = nativePtr;
			this.m_IsOwner = false;
		}

		~HierarchyCommandList()
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
					HierarchyCommandList.Destroy(this.m_Ptr);
				}
				this.m_Ptr = IntPtr.Zero;
			}
		}

		[NativeMethod(IsThreadSafe = true)]
		public void Clear()
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyCommandList.Clear_Injected(intPtr);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool Reserve(int count)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.Reserve_Injected(intPtr, count);
		}

		public bool Add(in HierarchyNode parent, out HierarchyNode node)
		{
			return this.AddNode(in parent, out node);
		}

		public bool Add(in HierarchyNode parent, int count, out HierarchyNode[] nodes)
		{
			bool flag = count < 0;
			if (flag)
			{
				throw new ArgumentException(string.Format("{0} must be positive, but was {1}", "count", count));
			}
			nodes = new HierarchyNode[count];
			return this.AddNodeSpan(in parent, nodes);
		}

		public bool Add(in HierarchyNode parent, Span<HierarchyNode> outNodes)
		{
			return this.AddNodeSpan(in parent, outNodes);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool Remove(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.Remove_Injected(intPtr, in node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool RemoveChildren(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.RemoveChildren_Injected(intPtr, in node);
		}

		public bool SetParent(in HierarchyNode node, in HierarchyNode parent)
		{
			return this.SetNodeParent(in node, in parent);
		}

		public bool SetParent(in HierarchyNode node, in HierarchyNode parent, int index)
		{
			return this.SetNodeParentAt(in node, in parent, index);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool SetSortIndex(in HierarchyNode node, int sortIndex)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.SetSortIndex_Injected(intPtr, in node, sortIndex);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool SortChildren(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.SortChildren_Injected(intPtr, in node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool SortChildrenRecursive(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.SortChildrenRecursive_Injected(intPtr, in node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool SetChildrenNeedsSorting(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.SetChildrenNeedsSorting_Injected(intPtr, in node);
		}

		public unsafe bool SetProperty<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(in HierarchyPropertyUnmanaged<T> property, in HierarchyNode node, T value) where T : struct, ValueType
		{
			return this.SetNodePropertyRaw(in property.m_Property, in node, (void*)(&value), sizeof(T));
		}

		public bool SetProperty(in HierarchyPropertyString property, in HierarchyNode node, string value)
		{
			return this.SetNodePropertyString(in property.m_Property, in node, value);
		}

		public bool ClearProperty<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(in HierarchyPropertyUnmanaged<T> property, in HierarchyNode node) where T : struct, ValueType
		{
			return this.ClearNodeProperty(in property.m_Property, in node);
		}

		public bool ClearProperty(in HierarchyPropertyString property, in HierarchyNode node)
		{
			return this.ClearNodeProperty(in property.m_Property, in node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public unsafe bool SetName(in HierarchyNode node, string name)
		{
			bool flag;
			try
			{
				IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = HierarchyCommandList.SetName_Injected(intPtr, in node, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[NativeMethod(IsThreadSafe = true)]
		public bool SetDirty()
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.SetDirty_Injected(intPtr);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public void Execute()
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyCommandList.Execute_Injected(intPtr);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool ExecuteIncremental()
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.ExecuteIncremental_Injected(intPtr);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool ExecuteIncrementalTimed(double milliseconds)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.ExecuteIncrementalTimed_Injected(intPtr, milliseconds);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static HierarchyCommandList FromIntPtr(IntPtr handlePtr)
		{
			return (handlePtr != IntPtr.Zero) ? ((HierarchyCommandList)GCHandle.FromIntPtr(handlePtr).Target) : null;
		}

		[FreeFunction("HierarchyCommandListBindings::Create", IsThreadSafe = true)]
		private static IntPtr Create(IntPtr handlePtr, Hierarchy hierarchy, HierarchyNodeType nodeType, int initialCapacity)
		{
			return HierarchyCommandList.Create_Injected(handlePtr, (hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy), ref nodeType, initialCapacity);
		}

		[FreeFunction("HierarchyCommandListBindings::Destroy", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy(IntPtr nativePtr);

		[FreeFunction("HierarchyCommandListBindings::AddNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private bool AddNode(in HierarchyNode parent, out HierarchyNode node)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.AddNode_Injected(intPtr, in parent, out node);
		}

		[FreeFunction("HierarchyCommandListBindings::AddNodeSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe bool AddNodeSpan(in HierarchyNode parent, Span<HierarchyNode> outNodes)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<HierarchyNode> span = outNodes;
			bool flag;
			fixed (HierarchyNode* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				flag = HierarchyCommandList.AddNodeSpan_Injected(intPtr, in parent, ref managedSpanWrapper);
			}
			return flag;
		}

		[FreeFunction("HierarchyCommandListBindings::SetNodeParent", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private bool SetNodeParent(in HierarchyNode node, in HierarchyNode parent)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.SetNodeParent_Injected(intPtr, in node, in parent);
		}

		[FreeFunction("HierarchyCommandListBindings::SetNodeParentAt", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private bool SetNodeParentAt(in HierarchyNode node, in HierarchyNode parent, int index)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.SetNodeParentAt_Injected(intPtr, in node, in parent, index);
		}

		[FreeFunction("HierarchyCommandListBindings::SetNodePropertyRaw", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe bool SetNodePropertyRaw(in HierarchyPropertyId property, in HierarchyNode node, void* ptr, int size)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.SetNodePropertyRaw_Injected(intPtr, in property, in node, ptr, size);
		}

		[FreeFunction("HierarchyCommandListBindings::SetNodePropertyString", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe bool SetNodePropertyString(in HierarchyPropertyId property, in HierarchyNode node, string value)
		{
			bool flag;
			try
			{
				IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = value.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = HierarchyCommandList.SetNodePropertyString_Injected(intPtr, in property, in node, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[FreeFunction("HierarchyCommandListBindings::ClearNodeProperty", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private bool ClearNodeProperty(in HierarchyPropertyId property, in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.ClearNodeProperty_Injected(intPtr, in property, in node);
		}

		[RequiredByNativeCode]
		private static IntPtr CreateCommandList(IntPtr nativePtr)
		{
			return GCHandle.ToIntPtr(GCHandle.Alloc(new HierarchyCommandList(nativePtr)));
		}

		[Obsolete("SortChildren(node, recurse) with a bool parameter is obsolete, please use SortChildren(node) or SortChildrenRecursive(node) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool SortChildren(in HierarchyNode node, bool recurse)
		{
			bool flag;
			if (recurse)
			{
				flag = this.SortChildrenRecursive(in node);
			}
			else
			{
				flag = this.SortChildren(in node);
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_Size_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_Capacity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_IsEmpty_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_IsExecuting_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Clear_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Reserve_Injected(IntPtr _unity_self, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Remove_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RemoveChildren_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetSortIndex_Injected(IntPtr _unity_self, in HierarchyNode node, int sortIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SortChildren_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SortChildrenRecursive_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetChildrenNeedsSorting_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetName_Injected(IntPtr _unity_self, in HierarchyNode node, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetDirty_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Execute_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ExecuteIncremental_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ExecuteIncrementalTimed_Injected(IntPtr _unity_self, double milliseconds);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create_Injected(IntPtr handlePtr, IntPtr hierarchy, [In] ref HierarchyNodeType nodeType, int initialCapacity);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddNode_Injected(IntPtr _unity_self, in HierarchyNode parent, out HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddNodeSpan_Injected(IntPtr _unity_self, in HierarchyNode parent, ref ManagedSpanWrapper outNodes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetNodeParent_Injected(IntPtr _unity_self, in HierarchyNode node, in HierarchyNode parent);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetNodeParentAt_Injected(IntPtr _unity_self, in HierarchyNode node, in HierarchyNode parent, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern bool SetNodePropertyRaw_Injected(IntPtr _unity_self, in HierarchyPropertyId property, in HierarchyNode node, void* ptr, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetNodePropertyString_Injected(IntPtr _unity_self, in HierarchyPropertyId property, in HierarchyNode node, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ClearNodeProperty_Injected(IntPtr _unity_self, in HierarchyPropertyId property, in HierarchyNode node);

		private IntPtr m_Ptr;

		private readonly bool m_IsOwner;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToUnmanaged(HierarchyCommandList cmdList)
			{
				return cmdList.m_Ptr;
			}
		}
	}
}
