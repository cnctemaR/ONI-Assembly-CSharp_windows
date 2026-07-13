using System;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.EnterLeave)]
	public sealed class PointerOverEvent : PointerEventBase<PointerOverEvent>
	{
		static PointerOverEvent()
		{
			EventBase<PointerOverEvent>.SetCreateFunction(() => new PointerOverEvent());
		}

		internal override void Dispatch(BaseVisualElementPanel panel)
		{
			EventDispatchUtilities.DispatchToAssignedTarget(this, panel);
		}
	}
}
