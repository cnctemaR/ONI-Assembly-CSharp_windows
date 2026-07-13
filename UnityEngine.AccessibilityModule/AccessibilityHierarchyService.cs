using System;
using System.Collections.Generic;

namespace UnityEngine.Accessibility
{
	internal static class AccessibilityHierarchyService
	{
		internal static AccessibilityHierarchy activeHierarchy
		{
			get
			{
				return AccessibilityHierarchyService.s_ActiveHierarchy;
			}
			set
			{
				bool flag = AccessibilityHierarchyService.s_ActiveHierarchy != value;
				if (flag)
				{
					AccessibilityHierarchy accessibilityHierarchy = AccessibilityHierarchyService.s_ActiveHierarchy;
					if (accessibilityHierarchy != null)
					{
						accessibilityHierarchy.FreeNative();
					}
					AccessibilityHierarchyService.s_ActiveHierarchy = value;
					AccessibilityHierarchy accessibilityHierarchy2 = AccessibilityHierarchyService.s_ActiveHierarchy;
					if (accessibilityHierarchy2 != null)
					{
						accessibilityHierarchy2.AllocateNative();
					}
					AssistiveSupport.notificationDispatcher.SendScreenChanged(null);
				}
			}
		}

		internal static IReadOnlyList<AccessibilityNode> GetRootNodes()
		{
			AccessibilityHierarchy accessibilityHierarchy = AccessibilityHierarchyService.s_ActiveHierarchy;
			return (accessibilityHierarchy != null) ? accessibilityHierarchy.rootNodes : null;
		}

		internal static bool TryGetNode(int id, out AccessibilityNode node)
		{
			node = null;
			AccessibilityHierarchy accessibilityHierarchy = AccessibilityHierarchyService.s_ActiveHierarchy;
			return accessibilityHierarchy != null && accessibilityHierarchy.TryGetNode(id, out node);
		}

		internal static bool TryGetNodeAt(float x, float y, out AccessibilityNode node)
		{
			node = null;
			AccessibilityHierarchy accessibilityHierarchy = AccessibilityHierarchyService.s_ActiveHierarchy;
			return accessibilityHierarchy != null && accessibilityHierarchy.TryGetNodeAt(x, y, out node);
		}

		private static AccessibilityHierarchy s_ActiveHierarchy;
	}
}
