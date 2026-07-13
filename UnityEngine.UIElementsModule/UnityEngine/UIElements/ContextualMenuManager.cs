using System;

namespace UnityEngine.UIElements
{
	public abstract class ContextualMenuManager
	{
		internal bool displayMenuHandledOSX { get; set; }

		public abstract void DisplayMenuIfEventMatches(EventBase evt, IEventHandler eventHandler);

		internal virtual bool CheckIfEventMatches(EventBase evt)
		{
			return false;
		}

		public void DisplayMenu(EventBase triggerEvent, IEventHandler target)
		{
			DropdownMenu dropdownMenu = new DropdownMenu();
			this.DisplayMenu(triggerEvent, target, dropdownMenu);
		}

		internal void DisplayMenu(EventBase triggerEvent, IEventHandler target, DropdownMenu menu)
		{
			int num;
			using (ContextualMenuPopulateEvent pooled = ContextualMenuPopulateEvent.GetPooled(triggerEvent, menu, target, this))
			{
				IPointerEvent pointerEvent = triggerEvent as IPointerEvent;
				num = ((pointerEvent != null) ? pointerEvent.pointerId : PointerId.mousePointerId);
				int button = pooled.button;
				if (target != null)
				{
					target.SendEvent(pooled);
				}
			}
			bool isOSXContextualMenuPlatform = UIElementsUtility.isOSXContextualMenuPlatform;
			if (isOSXContextualMenuPlatform)
			{
				this.displayMenuHandledOSX = true;
				ContextualMenuManager.ResetPointerDown(num);
			}
		}

		protected internal abstract void DoDisplayMenu(DropdownMenu menu, EventBase triggerEvent);

		internal static void ResetPointerDown(int pointerId)
		{
			PointerDeviceState.ReleaseAllButtons(pointerId);
		}

		internal void BeforePointerDown()
		{
			this.displayMenuHandledOSX = false;
		}

		internal void AfterPointerUp()
		{
			this.displayMenuHandledOSX = false;
		}
	}
}
