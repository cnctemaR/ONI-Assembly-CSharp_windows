using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "Unity.UIElements" })]
	[NativeHeader("Modules/UIElements/Core/Native/UIElementsRuntimeUtilityNative.h")]
	internal static class UIElementsRuntimeUtilityNative
	{
		[RequiredByNativeCode]
		public static void UpdatePanels()
		{
			Action updatePanelsCallback = UIElementsRuntimeUtilityNative.UpdatePanelsCallback;
			if (updatePanelsCallback != null)
			{
				updatePanelsCallback();
			}
		}

		[RequiredByNativeCode]
		public static void RepaintPanels(bool onlyOffscreen)
		{
			Action<bool> repaintPanelsCallback = UIElementsRuntimeUtilityNative.RepaintPanelsCallback;
			if (repaintPanelsCallback != null)
			{
				repaintPanelsCallback(onlyOffscreen);
			}
		}

		[RequiredByNativeCode]
		public static void RenderOffscreenPanels()
		{
			Action renderOffscreenPanelsCallback = UIElementsRuntimeUtilityNative.RenderOffscreenPanelsCallback;
			if (renderOffscreenPanelsCallback != null)
			{
				renderOffscreenPanelsCallback();
			}
		}

		public static void SetUpdateCallback(Action callback)
		{
			UIElementsRuntimeUtilityNative.UpdatePanelsCallback = callback;
		}

		public static void SetRenderingCallbacks(Action<bool> repaintPanels, Action renderOffscreenPanels)
		{
			UIElementsRuntimeUtilityNative.RepaintPanelsCallback = repaintPanels;
			UIElementsRuntimeUtilityNative.RenderOffscreenPanelsCallback = renderOffscreenPanels;
			UIElementsRuntimeUtilityNative.RegisterRenderingCallbacks();
		}

		public static void UnsetRenderingCallbacks()
		{
			UIElementsRuntimeUtilityNative.RepaintPanelsCallback = null;
			UIElementsRuntimeUtilityNative.RenderOffscreenPanelsCallback = null;
			UIElementsRuntimeUtilityNative.UnregisterRenderingCallbacks();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RegisterRenderingCallbacks();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UnregisterRenderingCallbacks();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VisualElementCreation();

		private static Action UpdatePanelsCallback;

		private static Action<bool> RepaintPanelsCallback;

		private static Action RenderOffscreenPanelsCallback;
	}
}
