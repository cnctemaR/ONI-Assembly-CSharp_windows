using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Hierarchy
{
	[NativeHeader("Modules/HierarchyCore/Public/HierarchyViewModel.h")]
	[NativeHeader("Modules/HierarchyCore/HierarchyViewModelBindings.h")]
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class HierarchyViewModel : IDisposable
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event HierarchyViewModel.FlagsChangedEventHandler FlagsChanged;

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
				return this.m_Nodes.Count;
			}
		}

		public bool Updating
		{
			[NativeMethod("Updating", IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HierarchyViewModel.get_Updating_Injected(intPtr);
			}
		}

		public bool UpdateNeeded
		{
			[NativeMethod("UpdateNeeded", IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HierarchyViewModel.get_UpdateNeeded_Injected(intPtr);
			}
		}

		public bool Filtering
		{
			[NativeMethod("Filtering", IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HierarchyViewModel.get_Filtering_Injected(intPtr);
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

		internal ReadOnlyNativeVector<HierarchyNode> Nodes
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Nodes;
			}
		}

		internal int Version
		{
			[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Version;
			}
		}

		internal float UpdateProgress
		{
			[NativeMethod("UpdateProgress", IsThreadSafe = true)]
			[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
			get
			{
				IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HierarchyViewModel.get_UpdateProgress_Injected(intPtr);
			}
		}

		internal IHierarchySearchQueryParser QueryParser
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
			get;
			[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
			set;
		}

		internal HierarchySearchQueryDescriptor Query
		{
			[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
			[NativeMethod(IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HierarchyViewModel.get_Query_Injected(intPtr);
			}
			[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
			[NativeMethod(IsThreadSafe = true)]
			set
			{
				IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				HierarchyViewModel.set_Query_Injected(intPtr, value);
			}
		}

		public HierarchyViewModel(HierarchyFlattened hierarchyFlattened, HierarchyNodeFlags defaultFlags = HierarchyNodeFlags.None)
		{
			IntPtr intPtr;
			int num;
			IntPtr intPtr2;
			int num2;
			int num3;
			this.m_Ptr = HierarchyViewModel.Create(GCHandle.ToIntPtr(GCHandle.Alloc(this)), hierarchyFlattened, defaultFlags, out intPtr, out num, out intPtr2, out num2, out num3);
			this.m_Hierarchy = hierarchyFlattened.m_Hierarchy;
			this.m_HierarchyFlattened = hierarchyFlattened;
			this.m_FlattenedNodes = new ReadOnlyNativeVector<HierarchyFlattenedNode>(intPtr, num);
			this.m_Nodes = new ReadOnlyNativeVector<HierarchyNode>(intPtr2, num2);
			this.m_Version = num3;
			this.m_IsOwner = true;
			this.QueryParser = new DefaultHierarchySearchQueryParser();
		}

		private HierarchyViewModel(IntPtr nativePtr, HierarchyFlattened hierarchyFlattened, IntPtr flattenedNodesPtr, int flattenedNodesCount, IntPtr nodesPtr, int nodesCount, int version)
		{
			this.m_Ptr = nativePtr;
			this.m_Hierarchy = hierarchyFlattened.m_Hierarchy;
			this.m_HierarchyFlattened = hierarchyFlattened;
			this.m_FlattenedNodes = new ReadOnlyNativeVector<HierarchyFlattenedNode>(flattenedNodesPtr, flattenedNodesCount);
			this.m_Nodes = new ReadOnlyNativeVector<HierarchyNode>(nodesPtr, nodesCount);
			this.m_Version = version;
			this.m_IsOwner = false;
			this.QueryParser = new DefaultHierarchySearchQueryParser();
		}

		~HierarchyViewModel()
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
					HierarchyViewModel.Destroy(this.m_Ptr);
				}
				this.m_Ptr = IntPtr.Zero;
			}
			this.m_FlattenedNodes = default(ReadOnlyNativeVector<HierarchyFlattenedNode>);
			this.m_Nodes = default(ReadOnlyNativeVector<HierarchyNode>);
		}

		public ref HierarchyNode this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Nodes[index];
			}
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int IndexOf(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.IndexOf_Injected(intPtr, in node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool Contains(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.Contains_Injected(intPtr, in node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public void SetRoot(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyViewModel.SetRoot_Injected(intPtr, in node);
		}

		[NativeMethod(IsThreadSafe = true)]
		public HierarchyNode GetRoot()
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyNode hierarchyNode;
			HierarchyViewModel.GetRoot_Injected(intPtr, out hierarchyNode);
			return hierarchyNode;
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public HierarchyNode GetParent(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyNode hierarchyNode;
			HierarchyViewModel.GetParent_Injected(intPtr, in node, out hierarchyNode);
			return hierarchyNode;
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public HierarchyNode GetNextSibling(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyNode hierarchyNode;
			HierarchyViewModel.GetNextSibling_Injected(intPtr, in node, out hierarchyNode);
			return hierarchyNode;
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int GetChildrenCount(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.GetChildrenCount_Injected(intPtr, in node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int GetChildrenCountRecursive(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.GetChildrenCountRecursive_Injected(intPtr, in node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public HierarchyNode GetChild(in HierarchyNode node, int index)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyNode hierarchyNode;
			HierarchyViewModel.GetChild_Injected(intPtr, in node, index, out hierarchyNode);
			return hierarchyNode;
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int GetChildIndex(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.GetChildIndex_Injected(intPtr, in node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int GetDepth(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.GetDepth_Injected(intPtr, in node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public HierarchyNodeFlags GetFlags(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.GetFlags_Injected(intPtr, in node);
		}

		public void SetFlags(HierarchyNodeFlags flags)
		{
			this.SetFlagsAll(flags);
		}

		public void SetFlags(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			this.SetFlagsNode(in node, flags);
		}

		public int SetFlags(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags)
		{
			return this.SetFlagsNodes(nodes, flags);
		}

		public int SetFlags(ReadOnlySpan<int> indices, HierarchyNodeFlags flags)
		{
			return this.SetFlagsIndices(indices, flags);
		}

		public void SetFlagsRecursive(in HierarchyNode node, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
		{
			this.SetFlagsRecursiveNode(in node, flags, direction);
		}

		public void SetFlagsRecursive(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
		{
			this.SetFlagsRecursiveNodes(nodes, flags, direction);
		}

		public bool HasAllFlags(HierarchyNodeFlags flags)
		{
			return this.HasAllFlagsAny(flags);
		}

		public bool HasAnyFlags(HierarchyNodeFlags flags)
		{
			return this.HasAnyFlagsAny(flags);
		}

		public bool HasAllFlags(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			return this.HasAllFlagsNode(in node, flags);
		}

		public bool HasAnyFlags(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			return this.HasAnyFlagsNode(in node, flags);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int HasAllFlagsCount(HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.HasAllFlagsCount_Injected(intPtr, flags);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int HasAnyFlagsCount(HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.HasAnyFlagsCount_Injected(intPtr, flags);
		}

		public bool DoesNotHaveAllFlags(HierarchyNodeFlags flags)
		{
			return this.DoesNotHaveAllFlagsAny(flags);
		}

		public bool DoesNotHaveAnyFlags(HierarchyNodeFlags flags)
		{
			return this.DoesNotHaveAnyFlagsAny(flags);
		}

		public bool DoesNotHaveAllFlags(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			return this.DoesNotHaveAllFlagsNode(in node, flags);
		}

		public bool DoesNotHaveAnyFlags(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			return this.DoesNotHaveAnyFlagsNode(in node, flags);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int DoesNotHaveAllFlagsCount(HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.DoesNotHaveAllFlagsCount_Injected(intPtr, flags);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int DoesNotHaveAnyFlagsCount(HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.DoesNotHaveAnyFlagsCount_Injected(intPtr, flags);
		}

		public void ClearFlags(HierarchyNodeFlags flags)
		{
			this.ClearFlagsAll(flags);
		}

		public void ClearFlags(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			this.ClearFlagsNode(in node, flags);
		}

		public int ClearFlags(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags)
		{
			return this.ClearFlagsNodes(nodes, flags);
		}

		public int ClearFlags(ReadOnlySpan<int> indices, HierarchyNodeFlags flags)
		{
			return this.ClearFlagsIndices(indices, flags);
		}

		public void ClearFlagsRecursive(in HierarchyNode node, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
		{
			this.ClearFlagsRecursiveNode(in node, flags, direction);
		}

		public void ClearFlagsRecursive(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
		{
			this.ClearFlagsRecursiveNodes(nodes, flags, direction);
		}

		public void ToggleFlags(HierarchyNodeFlags flags)
		{
			this.ToggleFlagsAll(flags);
		}

		public void ToggleFlags(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			this.ToggleFlagsNode(in node, flags);
		}

		public int ToggleFlags(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags)
		{
			return this.ToggleFlagsNodes(nodes, flags);
		}

		public int ToggleFlags(ReadOnlySpan<int> indices, HierarchyNodeFlags flags)
		{
			return this.ToggleFlagsIndices(indices, flags);
		}

		public void ToggleFlagsRecursive(in HierarchyNode node, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
		{
			this.ToggleFlagsRecursiveNode(in node, flags, direction);
		}

		public void ToggleFlagsRecursive(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
		{
			this.ToggleFlagsRecursiveNodes(nodes, flags, direction);
		}

		[NativeMethod(IsThreadSafe = true)]
		public void BeginFlagsChange()
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyViewModel.BeginFlagsChange_Injected(intPtr);
		}

		public HierarchyNodeFlags EndFlagsChange()
		{
			return this.EndFlagsChange(true);
		}

		public HierarchyNodeFlags EndFlagsChangeWithoutNotify()
		{
			return this.EndFlagsChange(false);
		}

		public int GetNodesWithAllFlags(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
		{
			return this.GetNodesWithAllFlagsSpan(flags, outNodes);
		}

		public int GetNodesWithAnyFlags(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
		{
			return this.GetNodesWithAnyFlagsSpan(flags, outNodes);
		}

		public HierarchyNode[] GetNodesWithAllFlags(HierarchyNodeFlags flags)
		{
			int num = this.HasAllFlagsCount(flags);
			bool flag = num == 0;
			HierarchyNode[] array;
			if (flag)
			{
				array = Array.Empty<HierarchyNode>();
			}
			else
			{
				HierarchyNode[] array2 = new HierarchyNode[num];
				this.GetNodesWithAllFlagsSpan(flags, array2);
				array = array2;
			}
			return array;
		}

		public HierarchyNode[] GetNodesWithAnyFlags(HierarchyNodeFlags flags)
		{
			int num = this.HasAnyFlagsCount(flags);
			bool flag = num == 0;
			HierarchyNode[] array;
			if (flag)
			{
				array = Array.Empty<HierarchyNode>();
			}
			else
			{
				HierarchyNode[] array2 = new HierarchyNode[num];
				this.GetNodesWithAnyFlagsSpan(flags, array2);
				array = array2;
			}
			return array;
		}

		public HierarchyViewModelNodesEnumerable EnumerateNodesWithAllFlags(HierarchyNodeFlags flags)
		{
			return new HierarchyViewModelNodesEnumerable(this, flags, new HierarchyViewModelNodesEnumerable.Predicate(this.HasAllFlags));
		}

		public HierarchyViewModelNodesEnumerable EnumerateNodesWithAnyFlags(HierarchyNodeFlags flags)
		{
			return new HierarchyViewModelNodesEnumerable(this, flags, new HierarchyViewModelNodesEnumerable.Predicate(this.HasAnyFlags));
		}

		public int GetIndicesWithAllFlags(HierarchyNodeFlags flags, Span<int> outIndices)
		{
			return this.GetIndicesWithAllFlagsSpan(flags, outIndices);
		}

		public int GetIndicesWithAnyFlags(HierarchyNodeFlags flags, Span<int> outIndices)
		{
			return this.GetIndicesWithAnyFlagsSpan(flags, outIndices);
		}

		public int[] GetIndicesWithAllFlags(HierarchyNodeFlags flags)
		{
			int num = this.HasAllFlagsCount(flags);
			bool flag = num == 0;
			int[] array;
			if (flag)
			{
				array = Array.Empty<int>();
			}
			else
			{
				int[] array2 = new int[num];
				this.GetIndicesWithAllFlagsSpan(flags, array2);
				array = array2;
			}
			return array;
		}

		public int[] GetIndicesWithAnyFlags(HierarchyNodeFlags flags)
		{
			int num = this.HasAnyFlagsCount(flags);
			bool flag = num == 0;
			int[] array;
			if (flag)
			{
				array = Array.Empty<int>();
			}
			else
			{
				int[] array2 = new int[num];
				this.GetIndicesWithAnyFlagsSpan(flags, array2);
				array = array2;
			}
			return array;
		}

		public int GetNodesWithoutAllFlags(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
		{
			return this.GetNodesWithoutAllFlagsSpan(flags, outNodes);
		}

		public int GetNodesWithoutAnyFlags(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
		{
			return this.GetNodesWithoutAnyFlagsSpan(flags, outNodes);
		}

		public HierarchyNode[] GetNodesWithoutAllFlags(HierarchyNodeFlags flags)
		{
			int num = this.DoesNotHaveAllFlagsCount(flags);
			bool flag = num == 0;
			HierarchyNode[] array;
			if (flag)
			{
				array = Array.Empty<HierarchyNode>();
			}
			else
			{
				HierarchyNode[] array2 = new HierarchyNode[num];
				this.GetNodesWithoutAllFlagsSpan(flags, array2);
				array = array2;
			}
			return array;
		}

		public HierarchyNode[] GetNodesWithoutAnyFlags(HierarchyNodeFlags flags)
		{
			int num = this.DoesNotHaveAnyFlagsCount(flags);
			bool flag = num == 0;
			HierarchyNode[] array;
			if (flag)
			{
				array = Array.Empty<HierarchyNode>();
			}
			else
			{
				HierarchyNode[] array2 = new HierarchyNode[num];
				this.GetNodesWithoutAnyFlagsSpan(flags, array2);
				array = array2;
			}
			return array;
		}

		public HierarchyViewModelNodesEnumerable EnumerateNodesWithoutAllFlags(HierarchyNodeFlags flags)
		{
			return new HierarchyViewModelNodesEnumerable(this, flags, new HierarchyViewModelNodesEnumerable.Predicate(this.DoesNotHaveAllFlags));
		}

		public HierarchyViewModelNodesEnumerable EnumerateNodesWithoutAnyFlags(HierarchyNodeFlags flags)
		{
			return new HierarchyViewModelNodesEnumerable(this, flags, new HierarchyViewModelNodesEnumerable.Predicate(this.DoesNotHaveAnyFlags));
		}

		public int GetIndicesWithoutAllFlags(HierarchyNodeFlags flags, Span<int> outIndices)
		{
			return this.GetIndicesWithoutAllFlagsSpan(flags, outIndices);
		}

		public int GetIndicesWithoutAnyFlags(HierarchyNodeFlags flags, Span<int> outIndices)
		{
			return this.GetIndicesWithoutAnyFlagsSpan(flags, outIndices);
		}

		public int[] GetIndicesWithoutAllFlags(HierarchyNodeFlags flags)
		{
			int num = this.DoesNotHaveAllFlagsCount(flags);
			bool flag = num == 0;
			int[] array;
			if (flag)
			{
				array = Array.Empty<int>();
			}
			else
			{
				int[] array2 = new int[num];
				this.GetIndicesWithoutAllFlagsSpan(flags, array2);
				array = array2;
			}
			return array;
		}

		public int[] GetIndicesWithoutAnyFlags(HierarchyNodeFlags flags)
		{
			int num = this.DoesNotHaveAnyFlagsCount(flags);
			bool flag = num == 0;
			int[] array;
			if (flag)
			{
				array = Array.Empty<int>();
			}
			else
			{
				int[] array2 = new int[num];
				this.GetIndicesWithoutAnyFlagsSpan(flags, array2);
				array = array2;
			}
			return array;
		}

		public void SetQuery(string query)
		{
			HierarchySearchQueryDescriptor hierarchySearchQueryDescriptor = this.QueryParser.ParseQuery(query);
			bool flag = hierarchySearchQueryDescriptor == this.Query;
			if (!flag)
			{
				this.Query = hierarchySearchQueryDescriptor;
			}
		}

		[NativeMethod(IsThreadSafe = true)]
		public void Update()
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyViewModel.Update_Injected(intPtr);
		}

		[NativeMethod(IsThreadSafe = true)]
		public bool UpdateIncremental()
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.UpdateIncremental_Injected(intPtr);
		}

		[NativeMethod(IsThreadSafe = true)]
		public bool UpdateIncrementalTimed(double milliseconds)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.UpdateIncrementalTimed_Injected(intPtr, milliseconds);
		}

		public HierarchyViewModel.Enumerator GetEnumerator()
		{
			return new HierarchyViewModel.Enumerator(this);
		}

		public ReadOnlySpan<HierarchyNode> AsReadOnlySpan()
		{
			return this.m_Nodes.AsReadOnlySpan();
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
		internal HierarchyViewModel.ReadOnlyList AsReadOnlyList()
		{
			return new HierarchyViewModel.ReadOnlyList(this);
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
		[FreeFunction("HierarchyViewModelBindings::GetState", HasExplicitThis = true, IsThreadSafe = true)]
		internal byte[] GetState()
		{
			byte[] array2;
			try
			{
				IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				HierarchyViewModel.GetState_Injected(intPtr, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				byte[] array;
				blittableArrayWrapper.Unmarshal<byte>(ref array);
				array2 = array;
			}
			return array2;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
		[FreeFunction("HierarchyViewModelBindings::SetState", HasExplicitThis = true, IsThreadSafe = true)]
		internal unsafe void SetState(ReadOnlySpan<byte> bytes)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<byte> readOnlySpan = bytes;
			fixed (byte* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				HierarchyViewModel.SetState_Injected(intPtr, ref managedSpanWrapper);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static HierarchyViewModel FromIntPtr(IntPtr handlePtr)
		{
			return (handlePtr != IntPtr.Zero) ? ((HierarchyViewModel)GCHandle.FromIntPtr(handlePtr).Target) : null;
		}

		[FreeFunction("HierarchyViewModelBindings::Create", IsThreadSafe = true)]
		private static IntPtr Create(IntPtr handlePtr, HierarchyFlattened hierarchyFlattened, HierarchyNodeFlags defaultFlags, out IntPtr nodesPtr, out int nodesCount, out IntPtr indicesPtr, out int indicesCount, out int version)
		{
			return HierarchyViewModel.Create_Injected(handlePtr, (hierarchyFlattened == null) ? ((IntPtr)0) : HierarchyFlattened.BindingsMarshaller.ConvertToUnmanaged(hierarchyFlattened), defaultFlags, out nodesPtr, out nodesCount, out indicesPtr, out indicesCount, out version);
		}

		[FreeFunction("HierarchyViewModelBindings::Destroy", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy(IntPtr nativePtr);

		[FreeFunction("HierarchyViewModelBindings::SetFlagsAll", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private void SetFlagsAll(HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyViewModel.SetFlagsAll_Injected(intPtr, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::SetFlagsNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private void SetFlagsNode(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyViewModel.SetFlagsNode_Injected(intPtr, in node, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::SetFlagsNodes", HasExplicitThis = true, IsThreadSafe = true)]
		private unsafe int SetFlagsNodes(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<HierarchyNode> readOnlySpan = nodes;
			int num;
			fixed (HierarchyNode* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				num = HierarchyViewModel.SetFlagsNodes_Injected(intPtr, ref managedSpanWrapper, flags);
			}
			return num;
		}

		[FreeFunction("HierarchyViewModelBindings::SetFlagsRecursiveNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private void SetFlagsRecursiveNode(in HierarchyNode node, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyViewModel.SetFlagsRecursiveNode_Injected(intPtr, in node, flags, direction);
		}

		[FreeFunction("HierarchyViewModelBindings::SetFlagsRecursiveNodes", HasExplicitThis = true, IsThreadSafe = true)]
		private unsafe void SetFlagsRecursiveNodes(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<HierarchyNode> readOnlySpan = nodes;
			fixed (HierarchyNode* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				HierarchyViewModel.SetFlagsRecursiveNodes_Injected(intPtr, ref managedSpanWrapper, flags, direction);
			}
		}

		[FreeFunction("HierarchyViewModelBindings::SetFlagsIndices", HasExplicitThis = true, IsThreadSafe = true)]
		private unsafe int SetFlagsIndices(ReadOnlySpan<int> indices, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<int> readOnlySpan = indices;
			int num;
			fixed (int* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				num = HierarchyViewModel.SetFlagsIndices_Injected(intPtr, ref managedSpanWrapper, flags);
			}
			return num;
		}

		[FreeFunction("HierarchyViewModelBindings::HasAllFlagsAny", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private bool HasAllFlagsAny(HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.HasAllFlagsAny_Injected(intPtr, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::HasAnyFlagsAny", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private bool HasAnyFlagsAny(HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.HasAnyFlagsAny_Injected(intPtr, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::HasAllFlagsNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private bool HasAllFlagsNode(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.HasAllFlagsNode_Injected(intPtr, in node, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::HasAnyFlagsNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private bool HasAnyFlagsNode(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.HasAnyFlagsNode_Injected(intPtr, in node, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::DoesNotHaveAllFlagsAny", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private bool DoesNotHaveAllFlagsAny(HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.DoesNotHaveAllFlagsAny_Injected(intPtr, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::DoesNotHaveAnyFlagsAny", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private bool DoesNotHaveAnyFlagsAny(HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.DoesNotHaveAnyFlagsAny_Injected(intPtr, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::DoesNotHaveAllFlagsNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private bool DoesNotHaveAllFlagsNode(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.DoesNotHaveAllFlagsNode_Injected(intPtr, in node, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::DoesNotHaveAnyFlagsNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private bool DoesNotHaveAnyFlagsNode(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.DoesNotHaveAnyFlagsNode_Injected(intPtr, in node, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::ClearFlagsAll", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private void ClearFlagsAll(HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyViewModel.ClearFlagsAll_Injected(intPtr, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::ClearFlagsNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private void ClearFlagsNode(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyViewModel.ClearFlagsNode_Injected(intPtr, in node, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::ClearFlagsNodes", HasExplicitThis = true, IsThreadSafe = true)]
		private unsafe int ClearFlagsNodes(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<HierarchyNode> readOnlySpan = nodes;
			int num;
			fixed (HierarchyNode* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				num = HierarchyViewModel.ClearFlagsNodes_Injected(intPtr, ref managedSpanWrapper, flags);
			}
			return num;
		}

		[FreeFunction("HierarchyViewModelBindings::ClearFlagsIndices", HasExplicitThis = true, IsThreadSafe = true)]
		private unsafe int ClearFlagsIndices(ReadOnlySpan<int> indices, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<int> readOnlySpan = indices;
			int num;
			fixed (int* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				num = HierarchyViewModel.ClearFlagsIndices_Injected(intPtr, ref managedSpanWrapper, flags);
			}
			return num;
		}

		[FreeFunction("HierarchyViewModelBindings::ClearFlagsRecursiveNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private void ClearFlagsRecursiveNode(in HierarchyNode node, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyViewModel.ClearFlagsRecursiveNode_Injected(intPtr, in node, flags, direction);
		}

		[FreeFunction("HierarchyViewModelBindings::ClearFlagsRecursiveNodes", HasExplicitThis = true, IsThreadSafe = true)]
		private unsafe void ClearFlagsRecursiveNodes(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<HierarchyNode> readOnlySpan = nodes;
			fixed (HierarchyNode* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				HierarchyViewModel.ClearFlagsRecursiveNodes_Injected(intPtr, ref managedSpanWrapper, flags, direction);
			}
		}

		[FreeFunction("HierarchyViewModelBindings::ToggleFlagsAll", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private void ToggleFlagsAll(HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyViewModel.ToggleFlagsAll_Injected(intPtr, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::ToggleFlagsNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private void ToggleFlagsNode(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyViewModel.ToggleFlagsNode_Injected(intPtr, in node, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::ToggleFlagsNodes", HasExplicitThis = true, IsThreadSafe = true)]
		private unsafe int ToggleFlagsNodes(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<HierarchyNode> readOnlySpan = nodes;
			int num;
			fixed (HierarchyNode* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				num = HierarchyViewModel.ToggleFlagsNodes_Injected(intPtr, ref managedSpanWrapper, flags);
			}
			return num;
		}

		[FreeFunction("HierarchyViewModelBindings::ToggleFlagsIndices", HasExplicitThis = true, IsThreadSafe = true)]
		private unsafe int ToggleFlagsIndices(ReadOnlySpan<int> indices, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<int> readOnlySpan = indices;
			int num;
			fixed (int* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				num = HierarchyViewModel.ToggleFlagsIndices_Injected(intPtr, ref managedSpanWrapper, flags);
			}
			return num;
		}

		[FreeFunction("HierarchyViewModelBindings::ToggleFlagsRecursiveNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private void ToggleFlagsRecursiveNode(in HierarchyNode node, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyViewModel.ToggleFlagsRecursiveNode_Injected(intPtr, in node, flags, direction);
		}

		[FreeFunction("HierarchyViewModelBindings::ToggleFlagsRecursiveNodes", HasExplicitThis = true, IsThreadSafe = true)]
		private unsafe void ToggleFlagsRecursiveNodes(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<HierarchyNode> readOnlySpan = nodes;
			fixed (HierarchyNode* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				HierarchyViewModel.ToggleFlagsRecursiveNodes_Injected(intPtr, ref managedSpanWrapper, flags, direction);
			}
		}

		[FreeFunction("HierarchyViewModelBindings::EndFlagsChange", HasExplicitThis = true, IsThreadSafe = true)]
		private HierarchyNodeFlags EndFlagsChange(bool notify)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.EndFlagsChange_Injected(intPtr, notify);
		}

		[FreeFunction("HierarchyViewModelBindings::GetNodesWithAllFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe int GetNodesWithAllFlagsSpan(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<HierarchyNode> span = outNodes;
			int nodesWithAllFlagsSpan_Injected;
			fixed (HierarchyNode* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				nodesWithAllFlagsSpan_Injected = HierarchyViewModel.GetNodesWithAllFlagsSpan_Injected(intPtr, flags, ref managedSpanWrapper);
			}
			return nodesWithAllFlagsSpan_Injected;
		}

		[FreeFunction("HierarchyViewModelBindings::GetNodesWithAnyFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe int GetNodesWithAnyFlagsSpan(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<HierarchyNode> span = outNodes;
			int nodesWithAnyFlagsSpan_Injected;
			fixed (HierarchyNode* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				nodesWithAnyFlagsSpan_Injected = HierarchyViewModel.GetNodesWithAnyFlagsSpan_Injected(intPtr, flags, ref managedSpanWrapper);
			}
			return nodesWithAnyFlagsSpan_Injected;
		}

		[FreeFunction("HierarchyViewModelBindings::GetIndicesWithAllFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe int GetIndicesWithAllFlagsSpan(HierarchyNodeFlags flags, Span<int> outIndices)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<int> span = outIndices;
			int indicesWithAllFlagsSpan_Injected;
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				indicesWithAllFlagsSpan_Injected = HierarchyViewModel.GetIndicesWithAllFlagsSpan_Injected(intPtr, flags, ref managedSpanWrapper);
			}
			return indicesWithAllFlagsSpan_Injected;
		}

		[FreeFunction("HierarchyViewModelBindings::GetIndicesWithAnyFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe int GetIndicesWithAnyFlagsSpan(HierarchyNodeFlags flags, Span<int> outIndices)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<int> span = outIndices;
			int indicesWithAnyFlagsSpan_Injected;
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				indicesWithAnyFlagsSpan_Injected = HierarchyViewModel.GetIndicesWithAnyFlagsSpan_Injected(intPtr, flags, ref managedSpanWrapper);
			}
			return indicesWithAnyFlagsSpan_Injected;
		}

		[FreeFunction("HierarchyViewModelBindings::GetNodesWithoutAllFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe int GetNodesWithoutAllFlagsSpan(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<HierarchyNode> span = outNodes;
			int nodesWithoutAllFlagsSpan_Injected;
			fixed (HierarchyNode* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				nodesWithoutAllFlagsSpan_Injected = HierarchyViewModel.GetNodesWithoutAllFlagsSpan_Injected(intPtr, flags, ref managedSpanWrapper);
			}
			return nodesWithoutAllFlagsSpan_Injected;
		}

		[FreeFunction("HierarchyViewModelBindings::GetNodesWithoutAnyFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe int GetNodesWithoutAnyFlagsSpan(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<HierarchyNode> span = outNodes;
			int nodesWithoutAnyFlagsSpan_Injected;
			fixed (HierarchyNode* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				nodesWithoutAnyFlagsSpan_Injected = HierarchyViewModel.GetNodesWithoutAnyFlagsSpan_Injected(intPtr, flags, ref managedSpanWrapper);
			}
			return nodesWithoutAnyFlagsSpan_Injected;
		}

		[FreeFunction("HierarchyViewModelBindings::GetIndicesWithoutAllFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe int GetIndicesWithoutAllFlagsSpan(HierarchyNodeFlags flags, Span<int> outIndices)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<int> span = outIndices;
			int indicesWithoutAllFlagsSpan_Injected;
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				indicesWithoutAllFlagsSpan_Injected = HierarchyViewModel.GetIndicesWithoutAllFlagsSpan_Injected(intPtr, flags, ref managedSpanWrapper);
			}
			return indicesWithoutAllFlagsSpan_Injected;
		}

		[FreeFunction("HierarchyViewModelBindings::GetIndicesWithoutAnyFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe int GetIndicesWithoutAnyFlagsSpan(HierarchyNodeFlags flags, Span<int> outIndices)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<int> span = outIndices;
			int indicesWithoutAnyFlagsSpan_Injected;
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				indicesWithoutAnyFlagsSpan_Injected = HierarchyViewModel.GetIndicesWithoutAnyFlagsSpan_Injected(intPtr, flags, ref managedSpanWrapper);
			}
			return indicesWithoutAnyFlagsSpan_Injected;
		}

		[RequiredByNativeCode]
		private static IntPtr CreateHierarchyViewModel(IntPtr nativePtr, IntPtr flattenedPtr, IntPtr flattenedNodesPtr, int flattenedNodesCount, IntPtr nodesPtr, int nodesCount, int version)
		{
			return GCHandle.ToIntPtr(GCHandle.Alloc(new HierarchyViewModel(nativePtr, HierarchyFlattened.FromIntPtr(flattenedPtr), flattenedNodesPtr, flattenedNodesCount, nodesPtr, nodesCount, version)));
		}

		[RequiredByNativeCode]
		private static void UpdateHierarchyViewModel(IntPtr handlePtr, IntPtr flattenedNodesPtr, int flattenedNodesCount, IntPtr nodesPtr, int nodesCount, int version)
		{
			HierarchyViewModel hierarchyViewModel = HierarchyViewModel.FromIntPtr(handlePtr);
			hierarchyViewModel.m_FlattenedNodes = new ReadOnlyNativeVector<HierarchyFlattenedNode>(flattenedNodesPtr, flattenedNodesCount);
			hierarchyViewModel.m_Nodes = new ReadOnlyNativeVector<HierarchyNode>(nodesPtr, nodesCount);
			hierarchyViewModel.m_Version = version;
		}

		[RequiredByNativeCode]
		private static void InvokeFlagsChanged(IntPtr handlePtr, HierarchyNodeFlags flags)
		{
			HierarchyViewModel hierarchyViewModel = HierarchyViewModel.FromIntPtr(handlePtr);
			HierarchyViewModel.FlagsChangedEventHandler flagsChanged = hierarchyViewModel.FlagsChanged;
			if (flagsChanged != null)
			{
				flagsChanged(flags);
			}
		}

		[RequiredByNativeCode]
		private static void SearchBegin(IntPtr handlePtr)
		{
			HierarchyViewModel hierarchyViewModel = HierarchyViewModel.FromIntPtr(handlePtr);
			foreach (HierarchyNodeTypeHandlerBase hierarchyNodeTypeHandlerBase in hierarchyViewModel.m_Hierarchy.EnumerateNodeTypeHandlersBase())
			{
				hierarchyNodeTypeHandlerBase.Internal_SearchBegin(hierarchyViewModel.Query);
			}
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

		[Obsolete("The HierarchyFlattened property will be removed in the future, remove its usage from your code.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public HierarchyFlattened HierarchyFlattned
		{
			get
			{
				return this.m_HierarchyFlattened;
			}
		}

		[Obsolete("SetFlags(node, flags, recurse) with a bool parameter is obsolete, please use SetFlags(node, flags) or SetFlags(node, flags, direction) instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetFlags(in HierarchyNode node, HierarchyNodeFlags flags, bool recurse)
		{
			if (recurse)
			{
				this.SetFlagsRecursiveNode(in node, flags, HierarchyTraversalDirection.Children);
			}
			else
			{
				this.SetFlagsNode(in node, flags);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("ClearFlags(node, flags, recurse) with a bool parameter is obsolete, please use ClearFlags(node, flags) or ClearFlags(node, flags, direction) instead.", false)]
		public void ClearFlags(in HierarchyNode node, HierarchyNodeFlags flags, bool recurse)
		{
			if (recurse)
			{
				this.ClearFlagsRecursiveNode(in node, flags, HierarchyTraversalDirection.Children);
			}
			else
			{
				this.ClearFlagsNode(in node, flags);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("ToggleFlags(node, flags, recurse) with a bool parameter is obsolete, please use ToggleFlags(node, flags) or ToggleFlags(node, flags, direction) instead.", false)]
		public void ToggleFlags(in HierarchyNode node, HierarchyNodeFlags flags, bool recurse)
		{
			if (recurse)
			{
				this.ToggleFlagsRecursiveNode(in node, flags, HierarchyTraversalDirection.Children);
			}
			else
			{
				this.ToggleFlagsNode(in node, flags);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("HasFlags is obsolete, please use HasAllFlags or HasAnyFlags instead.", false)]
		public bool HasFlags(HierarchyNodeFlags flags)
		{
			return this.HasAllFlagsAny(flags);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("HasFlags is obsolete, please use HasAllFlags or HasAnyFlags instead.", false)]
		public bool HasFlags(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			return this.HasAllFlagsNode(in node, flags);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("HasFlagsCount is obsolete, please use HasAllFlagsCount or HasAnyFlagsCount instead.", false)]
		public int HasFlagsCount(HierarchyNodeFlags flags)
		{
			return this.HasAllFlagsCount(flags);
		}

		[Obsolete("DoesNotHaveFlags is obsolete, please use DoesNotHaveAllFlags or DoesNotHaveAnyFlags instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool DoesNotHaveFlags(HierarchyNodeFlags flags)
		{
			return this.DoesNotHaveAllFlagsAny(flags);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("DoesNotHaveFlags is obsolete, please use DoesNotHaveAllFlags or DoesNotHaveAnyFlags instead.", false)]
		public bool DoesNotHaveFlags(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			return this.DoesNotHaveAllFlagsNode(in node, flags);
		}

		[Obsolete("DoesNotHaveFlagsCount is obsolete, please use DoesNotHaveAllFlagsCount or DoesNotHaveAnyFlagsCount instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int DoesNotHaveFlagsCount(HierarchyNodeFlags flags)
		{
			return this.DoesNotHaveAllFlagsCount(flags);
		}

		[Obsolete("GetNodesWithFlags is obsolete, please use GetNodesWithAllFlags or GetNodesWithAnyFlags instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int GetNodesWithFlags(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
		{
			return this.GetNodesWithAllFlagsSpan(flags, outNodes);
		}

		[Obsolete("GetNodesWithFlags is obsolete, please use GetNodesWithAllFlags or GetNodesWithAnyFlags instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public HierarchyNode[] GetNodesWithFlags(HierarchyNodeFlags flags)
		{
			return this.GetNodesWithAllFlags(flags);
		}

		[Obsolete("EnumerateNodesWithFlags is obsolete, please use EnumerateNodesWithAllFlags or EnumerateNodesWithAnyFlags instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public HierarchyViewModelNodesEnumerable EnumerateNodesWithFlags(HierarchyNodeFlags flags)
		{
			return this.EnumerateNodesWithAllFlags(flags);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("GetIndicesWithFlags is obsolete, please use GetIndicesWithAllFlags or GetIndicesWithAnyFlags instead.", false)]
		public int GetIndicesWithFlags(HierarchyNodeFlags flags, Span<int> outIndices)
		{
			return this.GetIndicesWithAllFlagsSpan(flags, outIndices);
		}

		[Obsolete("GetIndicesWithFlags is obsolete, please use GetIndicesWithAllFlags or GetIndicesWithAnyFlags instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int[] GetIndicesWithFlags(HierarchyNodeFlags flags)
		{
			return this.GetIndicesWithAllFlags(flags);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("GetNodesWithoutFlags is obsolete, please use GetNodesWithoutAllFlags or GetNodesWithoutAnyFlags instead.", false)]
		public int GetNodesWithoutFlags(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
		{
			return this.GetNodesWithoutAllFlagsSpan(flags, outNodes);
		}

		[Obsolete("GetNodesWithoutFlags is obsolete, please use GetNodesWithoutAllFlags or GetNodesWithoutAnyFlags instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public HierarchyNode[] GetNodesWithoutFlags(HierarchyNodeFlags flags)
		{
			return this.GetNodesWithoutAllFlags(flags);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("EnumerateNodesWithoutFlags is obsolete, please use EnumerateNodesWithoutAllFlags or EnumerateNodesWithoutAnyFlags instead.", false)]
		public HierarchyViewModelNodesEnumerable EnumerateNodesWithoutFlags(HierarchyNodeFlags flags)
		{
			return this.EnumerateNodesWithoutAllFlags(flags);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("GetIndicesWithoutFlags is obsolete, please use GetIndicesWithoutAllFlags or GetIndicesWithoutAnyFlags instead.", false)]
		public int GetIndicesWithoutFlags(HierarchyNodeFlags flags, Span<int> outIndices)
		{
			return this.GetIndicesWithoutAllFlagsSpan(flags, outIndices);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("GetIndicesWithoutFlags is obsolete, please use GetIndicesWithoutAllFlags or GetIndicesWithoutAnyFlags instead.", false)]
		public int[] GetIndicesWithoutFlags(HierarchyNodeFlags flags)
		{
			return this.GetIndicesWithoutAllFlags(flags);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_Updating_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_UpdateNeeded_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_Filtering_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_UpdateProgress_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern HierarchySearchQueryDescriptor get_Query_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_Query_Injected(IntPtr _unity_self, HierarchySearchQueryDescriptor value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int IndexOf_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Contains_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRoot_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRoot_Injected(IntPtr _unity_self, out HierarchyNode ret);

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
		private static extern HierarchyNodeFlags GetFlags_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int HasAllFlagsCount_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int HasAnyFlagsCount_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int DoesNotHaveAllFlagsCount_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int DoesNotHaveAnyFlagsCount_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BeginFlagsChange_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Update_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool UpdateIncremental_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool UpdateIncrementalTimed_Injected(IntPtr _unity_self, double milliseconds);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetState_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetState_Injected(IntPtr _unity_self, ref ManagedSpanWrapper bytes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create_Injected(IntPtr handlePtr, IntPtr hierarchyFlattened, HierarchyNodeFlags defaultFlags, out IntPtr nodesPtr, out int nodesCount, out IntPtr indicesPtr, out int indicesCount, out int version);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFlagsAll_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFlagsNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int SetFlagsNodes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper nodes, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFlagsRecursiveNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags, HierarchyTraversalDirection direction);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFlagsRecursiveNodes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper nodes, HierarchyNodeFlags flags, HierarchyTraversalDirection direction);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int SetFlagsIndices_Injected(IntPtr _unity_self, ref ManagedSpanWrapper indices, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasAllFlagsAny_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasAnyFlagsAny_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasAllFlagsNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasAnyFlagsNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool DoesNotHaveAllFlagsAny_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool DoesNotHaveAnyFlagsAny_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool DoesNotHaveAllFlagsNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool DoesNotHaveAnyFlagsNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearFlagsAll_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearFlagsNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ClearFlagsNodes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper nodes, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ClearFlagsIndices_Injected(IntPtr _unity_self, ref ManagedSpanWrapper indices, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearFlagsRecursiveNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags, HierarchyTraversalDirection direction);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearFlagsRecursiveNodes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper nodes, HierarchyNodeFlags flags, HierarchyTraversalDirection direction);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ToggleFlagsAll_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ToggleFlagsNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ToggleFlagsNodes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper nodes, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ToggleFlagsIndices_Injected(IntPtr _unity_self, ref ManagedSpanWrapper indices, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ToggleFlagsRecursiveNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags, HierarchyTraversalDirection direction);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ToggleFlagsRecursiveNodes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper nodes, HierarchyNodeFlags flags, HierarchyTraversalDirection direction);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern HierarchyNodeFlags EndFlagsChange_Injected(IntPtr _unity_self, bool notify);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetNodesWithAllFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outNodes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetNodesWithAnyFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outNodes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetIndicesWithAllFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outIndices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetIndicesWithAnyFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outIndices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetNodesWithoutAllFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outNodes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetNodesWithoutAnyFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outNodes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetIndicesWithoutAllFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outIndices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetIndicesWithoutAnyFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outIndices);

		private IntPtr m_Ptr;

		internal readonly Hierarchy m_Hierarchy;

		internal readonly HierarchyFlattened m_HierarchyFlattened;

		private ReadOnlyNativeVector<HierarchyFlattenedNode> m_FlattenedNodes;

		private ReadOnlyNativeVector<HierarchyNode> m_Nodes;

		private int m_Version;

		private readonly bool m_IsOwner;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToUnmanaged(HierarchyViewModel viewModel)
			{
				return viewModel.m_Ptr;
			}
		}

		public delegate void FlagsChangedEventHandler(HierarchyNodeFlags flags);

		public struct Enumerator
		{
			internal Enumerator(HierarchyViewModel hierarchyViewModel)
			{
				this.m_ViewModel = hierarchyViewModel;
				this.m_Nodes = hierarchyViewModel.m_Nodes;
				this.m_Version = hierarchyViewModel.m_Version;
				this.m_Index = -1;
			}

			public readonly ref HierarchyNode Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					bool flag = this.m_Version != this.m_ViewModel.m_Version;
					if (flag)
					{
						throw new InvalidOperationException("HierarchyViewModel was modified.");
					}
					return this.m_Nodes[this.m_Index];
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				int num = this.m_Index + 1;
				this.m_Index = num;
				return num < this.m_Nodes.Count;
			}

			private readonly HierarchyViewModel m_ViewModel;

			private readonly ReadOnlyNativeVector<HierarchyNode> m_Nodes;

			private readonly int m_Version;

			private int m_Index;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
		internal class ReadOnlyList : IList, ICollection, IEnumerable
		{
			internal ReadOnlyList(HierarchyViewModel viewModel)
			{
				this.m_ViewModel = viewModel;
			}

			public bool IsFixedSize
			{
				get
				{
					return true;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public int Count
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					if (!this.m_ViewModel.IsCreated)
					{
						throw new NullReferenceException("HierarchyViewModel has been disposed.");
					}
					return this.m_ViewModel.Count;
				}
			}

			public unsafe object this[int index]
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					if (!this.m_ViewModel.IsCreated)
					{
						throw new NullReferenceException("HierarchyViewModel has been disposed.");
					}
					return *this.m_ViewModel[index];
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			public bool Contains(object value)
			{
				HierarchyNode hierarchyNode;
				bool flag;
				if (value is HierarchyNode)
				{
					hierarchyNode = (HierarchyNode)value;
					flag = true;
				}
				else
				{
					flag = false;
				}
				bool flag2 = flag;
				bool flag3;
				if (flag2)
				{
					if (!this.m_ViewModel.IsCreated)
					{
						throw new NullReferenceException("HierarchyViewModel has been disposed.");
					}
					flag3 = this.m_ViewModel.Contains(in hierarchyNode);
				}
				else
				{
					flag3 = false;
				}
				return flag3;
			}

			public int IndexOf(object value)
			{
				HierarchyNode hierarchyNode;
				bool flag;
				if (value is HierarchyNode)
				{
					hierarchyNode = (HierarchyNode)value;
					flag = true;
				}
				else
				{
					flag = false;
				}
				bool flag2 = flag;
				int num;
				if (flag2)
				{
					if (!this.m_ViewModel.IsCreated)
					{
						throw new NullReferenceException("HierarchyViewModel has been disposed.");
					}
					num = this.m_ViewModel.IndexOf(in hierarchyNode);
				}
				else
				{
					num = -1;
				}
				return num;
			}

			public unsafe void CopyTo(Array array, int index)
			{
				for (int i = index; i < this.m_ViewModel.Count; i++)
				{
					array.SetValue(*this.m_ViewModel[i], i - index);
				}
			}

			public HierarchyViewModel.Enumerator GetEnumerator()
			{
				return new HierarchyViewModel.Enumerator(this.m_ViewModel);
			}

			int IList.Add(object value)
			{
				throw new NotSupportedException();
			}

			void IList.Clear()
			{
				throw new NotSupportedException();
			}

			void IList.Insert(int index, object value)
			{
				throw new NotSupportedException();
			}

			void IList.Remove(object value)
			{
				throw new NotSupportedException();
			}

			void IList.RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			void ICollection.CopyTo(Array array, int index)
			{
				throw new NotSupportedException();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				throw new NotSupportedException();
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			object ICollection.SyncRoot
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			private readonly HierarchyViewModel m_ViewModel;
		}
	}
}
