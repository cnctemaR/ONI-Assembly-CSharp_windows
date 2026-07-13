using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace Unity.Hierarchy
{
	[NativeHeader("Modules/HierarchyCore/HierarchyTestsHelper.h")]
	[NativeHeader("Modules/HierarchyCore/HierarchyTestsHelperBindings.h")]
	internal static class HierarchyTestsHelper
	{
		internal static int GenerateNodesTree(Hierarchy hierarchy, in HierarchyNode root, int width, int depth, int maxCount = 0)
		{
			return HierarchyTestsHelper.GenerateNodesTreeHierarchy(hierarchy, in root, width, depth, maxCount);
		}

		internal static int GenerateNodesTree(HierarchyNodeTypeHandlerBase handler, in HierarchyNode root, int width, int depth, int maxCount = 0)
		{
			return HierarchyTestsHelper.GenerateNodesTreeHandler(handler, in root, width, depth, maxCount);
		}

		internal static void GenerateNodesCount(Hierarchy hierarchy, in HierarchyNode root, int count, int width, int depth)
		{
			HierarchyTestsHelper.GenerateNodesCountHierarchy(hierarchy, in root, count, width, depth);
		}

		internal static void GenerateNodesCount(HierarchyNodeTypeHandlerBase handler, in HierarchyNode root, int count, int width, int depth)
		{
			HierarchyTestsHelper.GenerateNodesCountHandler(handler, in root, count, width, depth);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		internal static void GenerateSortIndexRecursive(Hierarchy hierarchy, in HierarchyNode root, HierarchyTestsHelper.SortOrder order)
		{
			HierarchyTestsHelper.GenerateSortIndexRecursive_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy), in root, order);
		}

		internal unsafe static void ForEachRecursive(Hierarchy hierarchy, in HierarchyNode root, HierarchyTestsHelper.ForEachCallback func)
		{
			Stack<HierarchyNode> stack = new Stack<HierarchyNode>();
			stack.Push(root);
			using (NativeArray<HierarchyNode> nativeArray = new NativeArray<HierarchyNode>(hierarchy.Count, Allocator.Temp, NativeArrayOptions.ClearMemory))
			{
				while (stack.Count > 0)
				{
					HierarchyNode hierarchyNode = stack.Pop();
					int childrenCount = hierarchy.GetChildrenCount(in hierarchyNode);
					Span<HierarchyNode> span = new Span<HierarchyNode>(nativeArray.GetUnsafePtr<HierarchyNode>(), childrenCount);
					int children = hierarchy.GetChildren(in hierarchyNode, span);
					bool flag = children != childrenCount;
					if (flag)
					{
						throw new InvalidOperationException(string.Format("Expected GetChildren to return {0}, but was {1}.", childrenCount, children));
					}
					int i = 0;
					int length = span.Length;
					while (i < length)
					{
						HierarchyNode hierarchyNode2 = *span[i];
						func(in hierarchyNode2, i);
						stack.Push(hierarchyNode2);
						i++;
					}
				}
			}
		}

		[NativeMethod(IsThreadSafe = true)]
		internal static byte[] GenerateInvalidViewModelState_BadIndices()
		{
			byte[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				HierarchyTestsHelper.GenerateInvalidViewModelState_BadIndices_Injected(out blittableArrayWrapper);
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

		[NativeMethod(IsThreadSafe = true)]
		internal static void SetNextHierarchyNodeId(Hierarchy hierarchy, int id)
		{
			HierarchyTestsHelper.SetNextHierarchyNodeId_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy), id);
		}

		internal static int GetNodeType<T>() where T : HierarchyNodeTypeHandlerBase
		{
			return HierarchyTestsHelper.GetNodeType(typeof(T));
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetNodeType(Type type);

		[NativeMethod(IsThreadSafe = true)]
		internal static int[] GetRegisteredNodeTypes(Hierarchy hierarchy)
		{
			int[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				HierarchyTestsHelper.GetRegisteredNodeTypes_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy), out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				int[] array;
				blittableArrayWrapper.Unmarshal<int>(ref array);
				array2 = array;
			}
			return array2;
		}

		[NativeMethod(IsThreadSafe = true)]
		internal static int GetCapacity(Hierarchy hierarchy)
		{
			return HierarchyTestsHelper.GetCapacity_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy));
		}

		internal static int GetVersion(Hierarchy hierarchy)
		{
			return HierarchyTestsHelper.GetHierarchyVersion(hierarchy);
		}

		internal static int GetVersion(Hierarchy hierarchy, in HierarchyNode node)
		{
			return HierarchyTestsHelper.GetHierarchyNodeVersion(hierarchy, in node);
		}

		internal static int GetVersion(HierarchyFlattened hierarchyFlattened)
		{
			return HierarchyTestsHelper.GetHierarchyFlattenedVersion(hierarchyFlattened);
		}

		internal static int GetVersion(HierarchyViewModel hierarchyViewModel)
		{
			return HierarchyTestsHelper.GetHierarchyViewModelVersion(hierarchyViewModel);
		}

		[NativeMethod(IsThreadSafe = true)]
		internal static int GetChildrenCapacity(Hierarchy hierarchy, in HierarchyNode node)
		{
			return HierarchyTestsHelper.GetChildrenCapacity_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy), in node);
		}

		[NativeMethod(IsThreadSafe = true)]
		internal static bool CompareNodeSortIndex(Hierarchy hierarchy, in HierarchyNode a, in HierarchyNode b)
		{
			return HierarchyTestsHelper.CompareNodeSortIndex_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy), in a, in b);
		}

		[NativeMethod(IsThreadSafe = true)]
		internal static object GetHierarchyScriptingObject(Hierarchy hierarchy)
		{
			return HierarchyTestsHelper.GetHierarchyScriptingObject_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy));
		}

		[NativeMethod(IsThreadSafe = true)]
		internal static object GetHierarchyFlattenedScriptingObject(HierarchyFlattened hierarchyFlattened)
		{
			return HierarchyTestsHelper.GetHierarchyFlattenedScriptingObject_Injected((hierarchyFlattened == null) ? ((IntPtr)0) : HierarchyFlattened.BindingsMarshaller.ConvertToUnmanaged(hierarchyFlattened));
		}

		[NativeMethod(IsThreadSafe = true)]
		internal static object GetHierarchyViewModelScriptingObject(HierarchyViewModel viewModel)
		{
			return HierarchyTestsHelper.GetHierarchyViewModelScriptingObject_Injected((viewModel == null) ? ((IntPtr)0) : HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(viewModel));
		}

		[NativeMethod(IsThreadSafe = true)]
		internal static object GetHierarchyCommandListScriptingObject(HierarchyCommandList cmdList)
		{
			return HierarchyTestsHelper.GetHierarchyCommandListScriptingObject_Injected((cmdList == null) ? ((IntPtr)0) : HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(cmdList));
		}

		[FreeFunction("HierarchyTestsHelperBindings::GenerateNodesTreeHierarchy", IsThreadSafe = true, ThrowsException = true)]
		private static int GenerateNodesTreeHierarchy(Hierarchy hierarchy, in HierarchyNode root, int width, int depth, int maxCount)
		{
			return HierarchyTestsHelper.GenerateNodesTreeHierarchy_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy), in root, width, depth, maxCount);
		}

		[FreeFunction("HierarchyTestsHelperBindings::GenerateNodesTreeHandler", IsThreadSafe = true, ThrowsException = true)]
		private static int GenerateNodesTreeHandler(HierarchyNodeTypeHandlerBase handler, in HierarchyNode root, int width, int depth, int maxCount)
		{
			return HierarchyTestsHelper.GenerateNodesTreeHandler_Injected((handler == null) ? ((IntPtr)0) : HierarchyNodeTypeHandlerBase.BindingsMarshaller.ConvertToUnmanaged(handler), in root, width, depth, maxCount);
		}

		[FreeFunction("HierarchyTestsHelperBindings::GenerateNodesCountHierarchy", IsThreadSafe = true, ThrowsException = true)]
		private static void GenerateNodesCountHierarchy(Hierarchy hierarchy, in HierarchyNode root, int count, int width, int depth)
		{
			HierarchyTestsHelper.GenerateNodesCountHierarchy_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy), in root, count, width, depth);
		}

		[FreeFunction("HierarchyTestsHelperBindings::GenerateNodesCountHandler", IsThreadSafe = true, ThrowsException = true)]
		private static void GenerateNodesCountHandler(HierarchyNodeTypeHandlerBase handler, in HierarchyNode root, int count, int width, int depth)
		{
			HierarchyTestsHelper.GenerateNodesCountHandler_Injected((handler == null) ? ((IntPtr)0) : HierarchyNodeTypeHandlerBase.BindingsMarshaller.ConvertToUnmanaged(handler), in root, count, width, depth);
		}

		[FreeFunction("HierarchyTestsHelperBindings::GetHierarchyVersion", IsThreadSafe = true)]
		private static int GetHierarchyVersion(Hierarchy hierarchy)
		{
			return HierarchyTestsHelper.GetHierarchyVersion_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy));
		}

		[FreeFunction("HierarchyTestsHelperBindings::GetHierarchyNodeVersion", IsThreadSafe = true)]
		private static int GetHierarchyNodeVersion(Hierarchy hierarchy, in HierarchyNode node)
		{
			return HierarchyTestsHelper.GetHierarchyNodeVersion_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy), in node);
		}

		[FreeFunction("HierarchyTestsHelperBindings::GetHierarchyFlattenedVersion", IsThreadSafe = true)]
		private static int GetHierarchyFlattenedVersion(HierarchyFlattened hierarchyFlattened)
		{
			return HierarchyTestsHelper.GetHierarchyFlattenedVersion_Injected((hierarchyFlattened == null) ? ((IntPtr)0) : HierarchyFlattened.BindingsMarshaller.ConvertToUnmanaged(hierarchyFlattened));
		}

		[FreeFunction("HierarchyTestsHelperBindings::GetHierarchyViewModelVersion", IsThreadSafe = true)]
		private static int GetHierarchyViewModelVersion(HierarchyViewModel hierarchyViewModel)
		{
			return HierarchyTestsHelper.GetHierarchyViewModelVersion_Injected((hierarchyViewModel == null) ? ((IntPtr)0) : HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(hierarchyViewModel));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GenerateSortIndexRecursive_Injected(IntPtr hierarchy, in HierarchyNode root, HierarchyTestsHelper.SortOrder order);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GenerateInvalidViewModelState_BadIndices_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetNextHierarchyNodeId_Injected(IntPtr hierarchy, int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRegisteredNodeTypes_Injected(IntPtr hierarchy, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetCapacity_Injected(IntPtr hierarchy);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetChildrenCapacity_Injected(IntPtr hierarchy, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CompareNodeSortIndex_Injected(IntPtr hierarchy, in HierarchyNode a, in HierarchyNode b);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object GetHierarchyScriptingObject_Injected(IntPtr hierarchy);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object GetHierarchyFlattenedScriptingObject_Injected(IntPtr hierarchyFlattened);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object GetHierarchyViewModelScriptingObject_Injected(IntPtr viewModel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object GetHierarchyCommandListScriptingObject_Injected(IntPtr cmdList);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GenerateNodesTreeHierarchy_Injected(IntPtr hierarchy, in HierarchyNode root, int width, int depth, int maxCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GenerateNodesTreeHandler_Injected(IntPtr handler, in HierarchyNode root, int width, int depth, int maxCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GenerateNodesCountHierarchy_Injected(IntPtr hierarchy, in HierarchyNode root, int count, int width, int depth);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GenerateNodesCountHandler_Injected(IntPtr handler, in HierarchyNode root, int count, int width, int depth);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetHierarchyVersion_Injected(IntPtr hierarchy);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetHierarchyNodeVersion_Injected(IntPtr hierarchy, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetHierarchyFlattenedVersion_Injected(IntPtr hierarchyFlattened);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetHierarchyViewModelVersion_Injected(IntPtr hierarchyViewModel);

		[NativeHeader("Modules/HierarchyCore/HierarchyTestsHelper.h")]
		internal enum SortOrder
		{
			Ascending,
			Descending
		}

		internal delegate void ForEachCallback(in HierarchyNode node, int index);
	}
}
