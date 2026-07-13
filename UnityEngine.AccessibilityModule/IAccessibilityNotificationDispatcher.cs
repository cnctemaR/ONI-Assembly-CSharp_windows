using System;

namespace UnityEngine.Accessibility
{
	public interface IAccessibilityNotificationDispatcher
	{
		void SendAnnouncement(string announcement);

		void SendPageScrolledAnnouncement(string announcement, AccessibilityNode nodeToFocus = null);

		void SendScreenChanged(AccessibilityNode nodeToFocus = null);

		void SendLayoutChanged(AccessibilityNode nodeToFocus = null);
	}
}
