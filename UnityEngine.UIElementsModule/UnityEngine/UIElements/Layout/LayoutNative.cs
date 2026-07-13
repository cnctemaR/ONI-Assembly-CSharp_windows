using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.UIElements.Layout
{
	[NativeHeader("Modules/UIElements/Core/Layout/Native/LayoutNative.h")]
	internal static class LayoutNative
	{
		[NativeMethod(IsThreadSafe = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void CalculateLayout(IntPtr node, float parentWidth, float parentHeight, int parentDirection, IntPtr state, IntPtr exceptionGCHandle);

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal static event Action<LayoutNative.LayoutLogData> onLayoutLog;

		[RequiredByNativeCode]
		private unsafe static void LayoutLog_Internal(IntPtr nodePtr, LayoutNative.LayoutLogEventType type, string message)
		{
			LayoutNative.LayoutLogData layoutLogData = new LayoutNative.LayoutLogData();
			layoutLogData.node = *(LayoutNode*)(void*)nodePtr;
			layoutLogData.message = message;
			layoutLogData.eventType = type;
			LayoutNative.onLayoutLog(layoutLogData);
		}

		internal enum LayoutLogEventType
		{
			None,
			Error,
			Measure,
			Layout,
			CacheUsage,
			BeginLayout,
			EndLayout
		}

		internal class LayoutLogData
		{
			public LayoutNode node;

			public LayoutNative.LayoutLogEventType eventType;

			public string message;
		}
	}
}
