using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Accessibility
{
	[NativeHeader("Modules/Accessibility/Native/AccessibilityNodeManager.h")]
	internal static class AccessibilityNodeManager
	{
		internal static bool CreateNativeNodeWithData(AccessibilityNodeData nodeData)
		{
			return AccessibilityNodeManager.CreateNativeNodeWithData_Injected(ref nodeData);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void DestroyNativeNode(int nodeId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetIsActive(int nodeId, bool isActive);

		internal unsafe static void SetLabel(int nodeId, string label)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(label, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = label.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				AccessibilityNodeManager.SetLabel_Injected(nodeId, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		internal unsafe static void SetValue(int nodeId, string value)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = value.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				AccessibilityNodeManager.SetValue_Injected(nodeId, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		internal unsafe static void SetHint(int nodeId, string hint)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(hint, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = hint.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				AccessibilityNodeManager.SetHint_Injected(nodeId, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetRole(int nodeId, AccessibilityRole role);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetAllowsDirectInteraction(int nodeId, bool allows);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetState(int nodeId, AccessibilityState state);

		internal static void SetFrame(int nodeId, Rect frame)
		{
			AccessibilityNodeManager.SetFrame_Injected(nodeId, ref frame);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetParent(int nodeId, int parentId, int index = -1);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetIsFocused(int nodeId);

		[RequiredByNativeCode]
		internal static void Internal_InvokeFocusChanged(int nodeId, bool isNodeFocused)
		{
			AccessibilityNode accessibilityNode;
			bool flag = AccessibilityHierarchyService.TryGetNode(nodeId, out accessibilityNode);
			if (flag)
			{
				accessibilityNode.NotifyFocusChanged(isNodeFocused);
			}
		}

		[RequiredByNativeCode]
		internal static bool Internal_InvokeNodeInvoked(int nodeId)
		{
			AccessibilityNode accessibilityNode;
			return AccessibilityHierarchyService.TryGetNode(nodeId, out accessibilityNode) && accessibilityNode.InvokeNodeInvoked();
		}

		[RequiredByNativeCode]
		internal static bool Internal_InvokeIncremented(int nodeId)
		{
			AccessibilityNode accessibilityNode;
			return AccessibilityHierarchyService.TryGetNode(nodeId, out accessibilityNode) && accessibilityNode.InvokeIncremented();
		}

		[RequiredByNativeCode]
		internal static bool Internal_InvokeDecremented(int nodeId)
		{
			AccessibilityNode accessibilityNode;
			return AccessibilityHierarchyService.TryGetNode(nodeId, out accessibilityNode) && accessibilityNode.InvokeDecremented();
		}

		[RequiredByNativeCode]
		internal static bool Internal_InvokeScrolled(int nodeId, AccessibilityScrollDirection direction)
		{
			AccessibilityNode accessibilityNode;
			return AccessibilityHierarchyService.TryGetNode(nodeId, out accessibilityNode) && accessibilityNode.InvokeScrolled(direction);
		}

		[RequiredByNativeCode]
		internal static bool Internal_InvokeDismissed(int nodeId)
		{
			AccessibilityNode accessibilityNode;
			return AccessibilityHierarchyService.TryGetNode(nodeId, out accessibilityNode) && accessibilityNode.InvokeDismissed();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateNativeNodeWithData_Injected([In] ref AccessibilityNodeData nodeData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLabel_Injected(int nodeId, ref ManagedSpanWrapper label);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetValue_Injected(int nodeId, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetHint_Injected(int nodeId, ref ManagedSpanWrapper hint);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFrame_Injected(int nodeId, [In] ref Rect frame);

		internal const int k_InvalidNodeId = -1;
	}
}
