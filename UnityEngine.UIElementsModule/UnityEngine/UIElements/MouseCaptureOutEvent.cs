using System;

namespace UnityEngine.UIElements
{
	public class MouseCaptureOutEvent : MouseCaptureEventBase<MouseCaptureOutEvent>
	{
		static MouseCaptureOutEvent()
		{
			EventBase<MouseCaptureOutEvent>.SetCreateFunction(() => new MouseCaptureOutEvent());
		}

		protected internal override void PreDispatch(IPanel panel)
		{
			base.PreDispatch(panel);
			base.elementTarget.UpdateCursorStyle(this.eventTypeId);
		}
	}
}
