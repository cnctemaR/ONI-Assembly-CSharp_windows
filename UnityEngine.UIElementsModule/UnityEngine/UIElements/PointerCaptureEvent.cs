using System;

namespace UnityEngine.UIElements
{
	public class PointerCaptureEvent : PointerCaptureEventBase<PointerCaptureEvent>
	{
		static PointerCaptureEvent()
		{
			EventBase<PointerCaptureEvent>.SetCreateFunction(() => new PointerCaptureEvent());
		}

		protected internal override void PreDispatch(IPanel panel)
		{
			base.PreDispatch(panel);
			base.elementTarget.UpdateHoverPseudoStateAfterCaptureChange(base.pointerId);
		}
	}
}
