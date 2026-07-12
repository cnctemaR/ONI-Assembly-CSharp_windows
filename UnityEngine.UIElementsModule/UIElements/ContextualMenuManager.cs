using System;

namespace UnityEngine.UIElements
{
	public abstract class ContextualMenuManager
	{
		internal bool displayMenuHandledOSX { get; set; }

		public abstract void DisplayMenuIfEventMatches(EventBase evt, IEventHandler eventHandler);

		public void DisplayMenu(EventBase triggerEvent, IEventHandler target)
		{
			DropdownMenu dropdownMenu = new DropdownMenu();
			int num;
			int button;
			using (ContextualMenuPopulateEvent pooled = ContextualMenuPopulateEvent.GetPooled(triggerEvent, dropdownMenu, target, this))
			{
				IPointerEvent pointerEvent = triggerEvent as IPointerEvent;
				num = ((pointerEvent != null) ? pointerEvent.pointerId : PointerId.mousePointerId);
				button = pooled.button;
				if (target != null)
				{
					target.SendEvent(pooled);
				}
			}
			bool flag = Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer;
			if (flag)
			{
				this.displayMenuHandledOSX = true;
				bool flag2 = button >= 0;
				if (flag2)
				{
					PointerDeviceState.ReleaseButton(num, button);
				}
			}
		}

		protected internal abstract void DoDisplayMenu(DropdownMenu menu, EventBase triggerEvent);
	}
}
