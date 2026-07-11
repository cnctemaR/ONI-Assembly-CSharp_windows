using System;

namespace UnityEngine.Experimental.UIElements
{
	public class BlurEvent : FocusEventBase<BlurEvent>
	{
		protected internal override void PreDispatch()
		{
			this.m_FocusController.DoFocusChange(base.relatedTarget);
		}
	}
}
