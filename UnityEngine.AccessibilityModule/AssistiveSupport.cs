using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.Bindings;

namespace UnityEngine.Accessibility
{
	public static class AssistiveSupport
	{
		public static IAccessibilityNotificationDispatcher notificationDispatcher { get; } = new AssistiveSupport.NotificationDispatcher();

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<bool> screenReaderStatusChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static event Action<AccessibilityHierarchy> s_ActiveHierarchyChanged;

		internal static event Action<AccessibilityHierarchy> activeHierarchyChanged
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.AccessibilityModule" })]
			add
			{
				AssistiveSupport.s_ActiveHierarchyChanged += value;
			}
			[VisibleToOtherModules(new string[] { "UnityEditor.AccessibilityModule" })]
			remove
			{
				AssistiveSupport.s_ActiveHierarchyChanged -= value;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<AccessibilityNode> nodeFocusChanged;

		public static AccessibilityHierarchy activeHierarchy
		{
			get
			{
				return AccessibilityHierarchyService.activeHierarchy;
			}
			set
			{
				bool flag = !Application.isEditor && !AccessibilityManager.isSupportedPlatform;
				if (flag)
				{
					Debug.LogError(string.Format("{0} is not supported on {1}. ", "activeHierarchy", Application.platform) + "Please refer to the documentation for supported platforms.");
				}
				else
				{
					bool flag2 = AssistiveSupport.isScreenReaderEnabled || Application.isEditor;
					if (flag2)
					{
						using (AccessibilityManager.GetExclusiveLock())
						{
							AccessibilityHierarchyService.activeHierarchy = value;
							Action<AccessibilityHierarchy> action = AssistiveSupport.s_ActiveHierarchyChanged;
							if (action != null)
							{
								action(value);
							}
						}
					}
				}
			}
		}

		public static bool isScreenReaderEnabled
		{
			get
			{
				AssistiveSupport.ScreenReaderStatusOverride screenReaderStatusOverride = AssistiveSupport.screenReaderStatusOverride;
				if (!true)
				{
				}
				bool flag = screenReaderStatusOverride == AssistiveSupport.ScreenReaderStatusOverride.ForceEnabled || (screenReaderStatusOverride != AssistiveSupport.ScreenReaderStatusOverride.ForceDisabled && AccessibilityManager.IsScreenReaderEnabled());
				if (!true)
				{
				}
				return flag;
			}
		}

		public static AssistiveSupport.ScreenReaderStatusOverride screenReaderStatusOverride
		{
			get
			{
				return AssistiveSupport.s_ScreenReaderStatusOverride;
			}
			set
			{
				bool flag = AssistiveSupport.s_ScreenReaderStatusOverride == value;
				if (!flag)
				{
					AssistiveSupport.s_ScreenReaderStatusOverride = value;
					bool flag2 = !AssistiveSupport.isScreenReaderEnabled && !Application.isEditor;
					if (flag2)
					{
						AccessibilityHierarchyService.activeHierarchy = null;
					}
				}
			}
		}

		[ExcludeFromCodeCoverage]
		internal static void Initialize()
		{
			AccessibilityManager.screenReaderStatusChanged += AssistiveSupport.ScreenReaderStatusChanged;
			AccessibilityManager.nodeFocusChanged += AssistiveSupport.NodeFocusChanged;
		}

		internal static void ScreenReaderStatusChanged(bool enabled)
		{
			bool flag = !AssistiveSupport.isScreenReaderEnabled && !Application.isEditor;
			if (flag)
			{
				AccessibilityHierarchyService.activeHierarchy = null;
			}
			Action<bool> action = AssistiveSupport.screenReaderStatusChanged;
			if (action != null)
			{
				action(enabled);
			}
		}

		private static void NodeFocusChanged(AccessibilityNode currentNode)
		{
			Action<AccessibilityNode> action = AssistiveSupport.nodeFocusChanged;
			if (action != null)
			{
				action(currentNode);
			}
		}

		private static AssistiveSupport.ScreenReaderStatusOverride s_ScreenReaderStatusOverride;

		internal class NotificationDispatcher : IAccessibilityNotificationDispatcher
		{
			public void SendAnnouncement(string announcement)
			{
				AccessibilityManager.SendAnnouncementNotification(announcement);
			}

			public void SendPageScrolledAnnouncement(string announcement, AccessibilityNode nodeToFocus = null)
			{
				AccessibilityManager.SendPageScrolledNotification(announcement, (nodeToFocus != null) ? nodeToFocus.id : (-1));
			}

			public void SendScreenChanged(AccessibilityNode nodeToFocus = null)
			{
				AccessibilityManager.SendScreenChangedNotification((nodeToFocus != null) ? nodeToFocus.id : (-1));
			}

			public void SendLayoutChanged(AccessibilityNode nodeToFocus = null)
			{
				AccessibilityManager.SendLayoutChangedNotification((nodeToFocus != null) ? nodeToFocus.id : (-1));
			}
		}

		public enum ScreenReaderStatusOverride : byte
		{
			OSDriven,
			ForceEnabled,
			ForceDisabled
		}
	}
}
