using System;

namespace UnityEngine.Experimental.UIElements
{
	public class FocusEvent : FocusEventBase<FocusEvent>
	{
		protected internal override void PreDispatch()
		{
			this.m_FocusController.DoFocusChange(base.target as Focusable);
		}
	}
}
