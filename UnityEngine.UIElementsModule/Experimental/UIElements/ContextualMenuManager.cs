using System;

namespace UnityEngine.Experimental.UIElements
{
	public abstract class ContextualMenuManager
	{
		public abstract void DisplayMenuIfEventMatches(EventBase evt, IEventHandler eventHandler);

		public void DisplayMenu(EventBase triggerEvent, IEventHandler target)
		{
			DropdownMenu dropdownMenu = new DropdownMenu();
			using (ContextualMenuPopulateEvent pooled = ContextualMenuPopulateEvent.GetPooled(triggerEvent, dropdownMenu, target, this))
			{
				target.SendEvent(pooled);
			}
		}

		protected internal abstract void DoDisplayMenu(DropdownMenu menu, EventBase triggerEvent);
	}
}
