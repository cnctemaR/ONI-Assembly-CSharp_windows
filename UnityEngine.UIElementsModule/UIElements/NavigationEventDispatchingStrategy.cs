using System;

namespace UnityEngine.UIElements
{
	internal class NavigationEventDispatchingStrategy : IEventDispatchingStrategy
	{
		public bool CanDispatchEvent(EventBase evt)
		{
			return evt is INavigationEvent;
		}

		public void DispatchEvent(EventBase evt, IPanel panel)
		{
			bool flag = panel != null;
			if (flag)
			{
				if (evt.target == null)
				{
					evt.target = panel.focusController.GetLeafFocusedElement() ?? panel.visualTree;
				}
				EventDispatchUtilities.PropagateEvent(evt);
			}
			evt.propagateToIMGUI = false;
			evt.stopDispatch = true;
		}
	}
}
