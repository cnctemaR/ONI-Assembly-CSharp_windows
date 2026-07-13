using System;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.EnterLeave)]
	public class MouseOutEvent : MouseEventBase<MouseOutEvent>
	{
		static MouseOutEvent()
		{
			EventBase<MouseOutEvent>.SetCreateFunction(() => new MouseOutEvent());
		}

		internal override void Dispatch(BaseVisualElementPanel panel)
		{
			EventDispatchUtilities.DispatchToAssignedTarget(this, panel);
		}

		protected internal override void PreDispatch(IPanel panel)
		{
			base.PreDispatch(panel);
			base.elementTarget.UpdateCursorStyle(this.eventTypeId);
		}
	}
}
