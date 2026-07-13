using System;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.EnterLeave)]
	public class MouseOverEvent : MouseEventBase<MouseOverEvent>
	{
		static MouseOverEvent()
		{
			EventBase<MouseOverEvent>.SetCreateFunction(() => new MouseOverEvent());
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
