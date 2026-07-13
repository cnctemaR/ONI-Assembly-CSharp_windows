using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Accessibility
{
	[VisibleToOtherModules(new string[] { "UnityEditor.AccessibilityModule" })]
	[NativeHeader("Modules/Accessibility/Native/AccessibilityNodeDataTests.h")]
	internal class AccessibilityNodeDataTests
	{
		[NativeThrows]
		internal static void Test_GetNodeDataToNativeViaBinding(AccessibilityNodeData nodeData)
		{
			AccessibilityNodeDataTests.Test_GetNodeDataToNativeViaBinding_Injected(ref nodeData);
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Test_GetNodeDataToNativeViaProxy();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Test_GetNodeDataFromNativeViaBinding(ref AccessibilityNodeData nodeData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Test_GetNodeDataFromNativeViaProxy();

		[RequiredByNativeCode]
		internal static void Internal_GetNodeDataFromManaged(ref AccessibilityNodeData nodeData)
		{
			nodeData = new AccessibilityNodeData
			{
				childIds = new int[] { 1, 2, 3 },
				label = "Label",
				value = "Value",
				hint = "Hint",
				frame = new Rect(10f, 20f, 100f, 200f),
				nodeId = 4,
				parentId = 5,
				role = AccessibilityRole.Button,
				state = AccessibilityState.Selected,
				isActive = true,
				allowsDirectInteraction = true,
				implementsInvoked = true,
				implementsScrolled = true,
				implementsDismissed = true
			};
		}

		[RequiredByNativeCode]
		internal static void Internal_GetNodeDataToManaged(in AccessibilityNodeData nodeData)
		{
			AccessibilityNodeDataTests.nodeDataFromNative = nodeData;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Test_GetNodeDataToNativeViaBinding_Injected([In] ref AccessibilityNodeData nodeData);

		internal static AccessibilityNodeData nodeDataFromNative;
	}
}
