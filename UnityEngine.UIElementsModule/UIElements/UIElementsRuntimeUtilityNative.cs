using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.UIElements
{
	[NativeHeader("ModuleOverrides/com.unity.ui/Core/Native/UIElementsRuntimeUtilityNative.h")]
	[VisibleToOtherModules(new string[] { "Unity.UIElements" })]
	internal static class UIElementsRuntimeUtilityNative
	{
		[RequiredByNativeCode]
		public static void RepaintOverlayPanels()
		{
			Action repaintOverlayPanelsCallback = UIElementsRuntimeUtilityNative.RepaintOverlayPanelsCallback;
			if (repaintOverlayPanelsCallback != null)
			{
				repaintOverlayPanelsCallback();
			}
		}

		[RequiredByNativeCode]
		public static void UpdateRuntimePanels()
		{
			Action updateRuntimePanelsCallback = UIElementsRuntimeUtilityNative.UpdateRuntimePanelsCallback;
			if (updateRuntimePanelsCallback != null)
			{
				updateRuntimePanelsCallback();
			}
		}

		[RequiredByNativeCode]
		public static void RepaintOffscreenPanels()
		{
			Action repaintOffscreenPanelsCallback = UIElementsRuntimeUtilityNative.RepaintOffscreenPanelsCallback;
			if (repaintOffscreenPanelsCallback != null)
			{
				repaintOffscreenPanelsCallback();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RegisterPlayerloopCallback();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UnregisterPlayerloopCallback();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VisualElementCreation();

		internal static Action RepaintOverlayPanelsCallback;

		internal static Action UpdateRuntimePanelsCallback;

		internal static Action RepaintOffscreenPanelsCallback;
	}
}
