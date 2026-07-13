using System;

namespace UnityEngine.UIElements
{
	public class PointerCaptureOutEvent : PointerCaptureEventBase<PointerCaptureOutEvent>
	{
		static PointerCaptureOutEvent()
		{
			EventBase<PointerCaptureOutEvent>.SetCreateFunction(() => new PointerCaptureOutEvent());
		}

		protected internal override void PreDispatch(IPanel panel)
		{
			base.PreDispatch(panel);
			base.elementTarget.UpdateHoverPseudoStateAfterCaptureChange(base.pointerId);
		}
	}
}
