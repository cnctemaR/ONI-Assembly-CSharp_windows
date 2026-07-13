using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Accessibility
{
	[NativeHeader("Modules/Accessibility/Native/AccessibilitySettings.h")]
	public static class AccessibilitySettings
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetFontScale();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsBoldTextEnabled();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsClosedCaptioningEnabled();

		[RequiredByNativeCode]
		internal static void Internal_OnFontScaleChanged(float newFontScale)
		{
			AccessibilityManager.QueueNotification(new AccessibilityManager.NotificationContext
			{
				notification = AccessibilityManager.Notification.FontScaleChanged,
				fontScale = newFontScale
			});
		}

		[RequiredByNativeCode]
		internal static void Internal_OnBoldTextStatusChanged(bool enabled)
		{
			AccessibilityManager.QueueNotification(new AccessibilityManager.NotificationContext
			{
				notification = AccessibilityManager.Notification.BoldTextStatusChanged,
				isBoldTextEnabled = enabled
			});
		}

		[RequiredByNativeCode]
		internal static void Internal_OnClosedCaptioningStatusChanged(bool enabled)
		{
			AccessibilityManager.QueueNotification(new AccessibilityManager.NotificationContext
			{
				notification = AccessibilityManager.Notification.ClosedCaptioningStatusChanged,
				isClosedCaptioningEnabled = enabled
			});
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<float> fontScaleChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<bool> boldTextStatusChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<bool> closedCaptioningStatusChanged;

		public static float fontScale
		{
			get
			{
				return AccessibilitySettings.GetFontScale();
			}
		}

		public static bool isBoldTextEnabled
		{
			get
			{
				return AccessibilitySettings.IsBoldTextEnabled();
			}
		}

		public static bool isClosedCaptioningEnabled
		{
			get
			{
				return AccessibilitySettings.IsClosedCaptioningEnabled();
			}
		}

		internal static void InvokeFontScaleChanged(float newFontScale)
		{
			Action<float> action = AccessibilitySettings.fontScaleChanged;
			if (action != null)
			{
				action(newFontScale);
			}
		}

		internal static void InvokeBoldTextStatusChanged(bool enabled)
		{
			Action<bool> action = AccessibilitySettings.boldTextStatusChanged;
			if (action != null)
			{
				action(enabled);
			}
		}

		internal static void InvokeClosedCaptionStatusChanged(bool enabled)
		{
			Action<bool> action = AccessibilitySettings.closedCaptioningStatusChanged;
			if (action != null)
			{
				action(enabled);
			}
		}
	}
}
