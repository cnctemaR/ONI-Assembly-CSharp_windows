using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Pool;
using UnityEngine.Scripting;

namespace UnityEngine.Accessibility
{
	[VisibleToOtherModules(new string[] { "UnityEditor.AccessibilityModule" })]
	[NativeHeader("Modules/Accessibility/Native/AccessibilityManager.h")]
	internal class AccessibilityManager
	{
		private AccessibilityManager()
		{
		}

		public static AccessibilityManager instance
		{
			get
			{
				return AccessibilityManager.Nested.s_Instance;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<bool> screenReaderStatusChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<AccessibilityNode> nodeFocusChanged;

		public static bool isSupportedPlatform
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				return platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.OSXPlayer || platform == RuntimePlatform.WindowsPlayer;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsScreenReaderEnabled();

		internal unsafe static void SendAnnouncementNotification(string announcement)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(announcement, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = announcement.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				AccessibilityManager.SendAnnouncementNotification_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		internal unsafe static void SendPageScrolledNotification(string announcement, int nodeId = -1)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(announcement, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = announcement.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				AccessibilityManager.SendPageScrolledNotification_Injected(ref managedSpanWrapper, nodeId);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SendScreenChangedNotification(int nodeId = -1);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SendLayoutChangedNotification(int nodeId = -1);

		[ExcludeFromCodeCoverage]
		[VisibleToOtherModules(new string[] { "UnityEditor.AccessibilityModule" })]
		[RequiredByNativeCode]
		internal static void Internal_Initialize()
		{
			AssistiveSupport.Initialize();
		}

		[RequiredByNativeCode]
		internal static void Internal_Update()
		{
			AccessibilityManager.instance.Internal_Update_Impl();
		}

		private void Internal_Update_Impl()
		{
			bool flag = AccessibilityManager.asyncNotificationContexts.Count == 0;
			if (!flag)
			{
				Queue<AccessibilityManager.NotificationContext> queue = AccessibilityManager.asyncNotificationContexts;
				AccessibilityManager.NotificationContext[] array;
				lock (queue)
				{
					bool flag3 = AccessibilityManager.asyncNotificationContexts.Count == 0;
					if (flag3)
					{
						return;
					}
					array = AccessibilityManager.asyncNotificationContexts.ToArray();
					AccessibilityManager.asyncNotificationContexts.Clear();
				}
				using (AccessibilityManager.GetExclusiveLock())
				{
					foreach (AccessibilityManager.NotificationContext notificationContext in array)
					{
						switch (notificationContext.notification)
						{
						case AccessibilityManager.Notification.ScreenReaderStatusChanged:
						{
							Action<bool> action = AccessibilityManager.screenReaderStatusChanged;
							if (action != null)
							{
								action(notificationContext.isScreenReaderEnabled);
							}
							break;
						}
						case AccessibilityManager.Notification.ElementFocused:
						{
							notificationContext.focusedNode.InvokeFocusChanged(true);
							Action<AccessibilityNode> action2 = AccessibilityManager.nodeFocusChanged;
							if (action2 != null)
							{
								action2(notificationContext.focusedNode);
							}
							break;
						}
						case AccessibilityManager.Notification.ElementUnfocused:
							notificationContext.focusedNode.InvokeFocusChanged(false);
							break;
						case AccessibilityManager.Notification.FontScaleChanged:
							AccessibilitySettings.InvokeFontScaleChanged(notificationContext.fontScale);
							break;
						case AccessibilityManager.Notification.BoldTextStatusChanged:
							AccessibilitySettings.InvokeBoldTextStatusChanged(notificationContext.isBoldTextEnabled);
							break;
						case AccessibilityManager.Notification.ClosedCaptioningStatusChanged:
							AccessibilitySettings.InvokeClosedCaptionStatusChanged(notificationContext.isClosedCaptioningEnabled);
							break;
						}
					}
				}
			}
		}

		[RequiredByNativeCode]
		internal static void Internal_LateUpdate()
		{
			bool refreshNodeFramesRequested = AccessibilityManager.instance.m_RefreshNodeFramesRequested;
			if (refreshNodeFramesRequested)
			{
				AccessibilityManager.instance.m_RefreshNodeFramesRequested = false;
				AccessibilityHierarchy activeHierarchy = AssistiveSupport.activeHierarchy;
				if (activeHierarchy != null)
				{
					activeHierarchy.RefreshNodeFrames();
				}
			}
		}

		[RequiredByNativeCode]
		internal static int[] Internal_GetRootNodeIds()
		{
			IReadOnlyList<AccessibilityNode> rootNodes = AccessibilityHierarchyService.GetRootNodes();
			bool flag = rootNodes == null || rootNodes.Count == 0;
			int[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				List<int> list;
				using (CollectionPool<List<int>, int>.Get(out list))
				{
					foreach (AccessibilityNode accessibilityNode in rootNodes)
					{
						list.Add(accessibilityNode.id);
					}
					array = ((list.Count == 0) ? null : list.ToArray());
				}
			}
			return array;
		}

		[RequiredByNativeCode]
		internal static bool Internal_GetNode(int nodeId, ref AccessibilityNodeData nodeData)
		{
			AccessibilityNode accessibilityNode;
			bool flag = !AccessibilityHierarchyService.TryGetNode(nodeId, out accessibilityNode);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				nodeData = new AccessibilityNodeData();
				accessibilityNode.GetNodeData(ref nodeData);
				flag2 = true;
			}
			return flag2;
		}

		[RequiredByNativeCode]
		internal static int Internal_GetNodeIdAt(float x, float y)
		{
			AccessibilityNode accessibilityNode;
			return AccessibilityHierarchyService.TryGetNodeAt(x, y, out accessibilityNode) ? accessibilityNode.id : (-1);
		}

		[RequiredByNativeCode]
		internal static bool Internal_GetFirstOrLastRootNodeId(bool first, out int managedRootId)
		{
			managedRootId = -1;
			IReadOnlyList<AccessibilityNode> rootNodes = AccessibilityHierarchyService.GetRootNodes();
			bool flag = rootNodes == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = rootNodes.Count != 0;
				if (flag3)
				{
					int num;
					if (!first)
					{
						IReadOnlyList<AccessibilityNode> readOnlyList = rootNodes;
						num = readOnlyList[readOnlyList.Count - 1].id;
					}
					else
					{
						num = rootNodes[0].id;
					}
					managedRootId = num;
				}
				flag2 = true;
			}
			return flag2;
		}

		[RequiredByNativeCode]
		internal static bool Internal_GetFirstOrLastChildId(int nodeId, bool first, out int childId)
		{
			childId = -1;
			AccessibilityNode accessibilityNode;
			bool flag = !AccessibilityHierarchyService.TryGetNode(nodeId, out accessibilityNode);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = accessibilityNode.children.Count != 0;
				if (flag3)
				{
					int num;
					if (!first)
					{
						IReadOnlyList<AccessibilityNode> children = accessibilityNode.children;
						num = children[children.Count - 1].id;
					}
					else
					{
						num = accessibilityNode.children[0].id;
					}
					childId = num;
				}
				flag2 = true;
			}
			return flag2;
		}

		[RequiredByNativeCode]
		internal static bool Internal_GetNextOrPreviousSiblingId(int nodeId, bool next, out int siblingId)
		{
			siblingId = -1;
			AccessibilityNode accessibilityNode;
			bool flag = !AccessibilityHierarchyService.TryGetNode(nodeId, out accessibilityNode);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				AccessibilityNode parent = accessibilityNode.parent;
				IReadOnlyList<AccessibilityNode> readOnlyList = ((parent != null) ? parent.children : null) ?? AccessibilityHierarchyService.GetRootNodes();
				bool flag3 = readOnlyList == null || readOnlyList.Count == 0;
				if (flag3)
				{
					throw new ArgumentException((accessibilityNode.parent == null) ? string.Format("Node with ID {0} without parent is not tracked as a root.", nodeId) : string.Format("Node with ID {0} is not a child of its parent.", nodeId));
				}
				bool flag4 = readOnlyList.Count == 1;
				if (flag4)
				{
					flag2 = true;
				}
				else
				{
					int num = AccessibilityManager.<Internal_GetNextOrPreviousSiblingId>g__IndexOf|30_0<AccessibilityNode>(accessibilityNode, readOnlyList);
					int num2 = (next ? (num + 1) : (num - 1));
					siblingId = ((num2 >= 0 && num2 < readOnlyList.Count) ? readOnlyList[num2].id : (-1));
					flag2 = true;
				}
			}
			return flag2;
		}

		[RequiredByNativeCode]
		internal static void Internal_OnScreenReaderStatusChanged(bool enabled)
		{
			AccessibilityManager.QueueNotification(new AccessibilityManager.NotificationContext
			{
				notification = AccessibilityManager.Notification.ScreenReaderStatusChanged,
				isScreenReaderEnabled = enabled
			});
		}

		[RequiredByNativeCode]
		internal static void Internal_OnWindowGeometryChanged()
		{
			AccessibilityManager.instance.m_RefreshNodeFramesRequested = true;
		}

		internal static void QueueNotification(AccessibilityManager.NotificationContext notification)
		{
			AccessibilityManager.instance.QueueNotification_Impl(notification);
		}

		internal void QueueNotification_Impl(AccessibilityManager.NotificationContext notification)
		{
			Queue<AccessibilityManager.NotificationContext> queue = AccessibilityManager.asyncNotificationContexts;
			lock (queue)
			{
				AccessibilityManager.asyncNotificationContexts.Enqueue(notification);
			}
		}

		internal static IDisposable GetExclusiveLock()
		{
			return new AccessibilityManager.ExclusiveLock();
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Lock();

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Unlock();

		[CompilerGenerated]
		internal static int <Internal_GetNextOrPreviousSiblingId>g__IndexOf|30_0<T>(T elementToFind, IReadOnlyList<T> list)
		{
			int num = 0;
			foreach (T t in list)
			{
				bool flag = object.Equals(t, elementToFind);
				if (flag)
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SendAnnouncementNotification_Injected(ref ManagedSpanWrapper announcement);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SendPageScrolledNotification_Injected(ref ManagedSpanWrapper announcement, int nodeId);

		internal static Queue<AccessibilityManager.NotificationContext> asyncNotificationContexts = new Queue<AccessibilityManager.NotificationContext>();

		private bool m_RefreshNodeFramesRequested;

		public enum Notification : byte
		{
			None,
			ScreenReaderStatusChanged,
			ElementFocused,
			ElementUnfocused,
			FontScaleChanged,
			BoldTextStatusChanged,
			ClosedCaptioningStatusChanged
		}

		public struct NotificationContext
		{
			public AccessibilityNode focusedNode { readonly get; set; }

			public float fontScale { readonly get; set; }

			public bool isBoldTextEnabled { readonly get; set; }

			public bool isClosedCaptioningEnabled { readonly get; set; }

			public bool isScreenReaderEnabled { readonly get; set; }

			public AccessibilityManager.Notification notification { readonly get; set; }
		}

		private class Nested
		{
			internal static readonly AccessibilityManager s_Instance = new AccessibilityManager();
		}

		private sealed class ExclusiveLock : IDisposable
		{
			public ExclusiveLock()
			{
				AccessibilityManager.Lock();
			}

			~ExclusiveLock()
			{
				this.InternalDispose();
			}

			private void InternalDispose()
			{
				bool flag = !this.m_Disposed;
				if (flag)
				{
					AccessibilityManager.Unlock();
					this.m_Disposed = true;
				}
			}

			public void Dispose()
			{
				this.InternalDispose();
				GC.SuppressFinalize(this);
			}

			private bool m_Disposed;
		}
	}
}
