using System;

namespace UnityEngine.UIElements
{
	public class FocusEvent : FocusEventBase<FocusEvent>
	{
		static FocusEvent()
		{
			EventBase<FocusEvent>.SetCreateFunction(() => new FocusEvent());
		}

		protected internal override void PreDispatch(IPanel panel)
		{
			base.PreDispatch(panel);
			base.focusController.ProcessPendingFocusChange(base.target as Focusable);
		}
	}
}
