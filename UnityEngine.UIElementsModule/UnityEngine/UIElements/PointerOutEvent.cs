using System;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.EnterLeave)]
	public sealed class PointerOutEvent : PointerEventBase<PointerOutEvent>
	{
		static PointerOutEvent()
		{
			EventBase<PointerOutEvent>.SetCreateFunction(() => new PointerOutEvent());
		}

		internal override void Dispatch(BaseVisualElementPanel panel)
		{
			EventDispatchUtilities.DispatchToAssignedTarget(this, panel);
		}
	}
}
