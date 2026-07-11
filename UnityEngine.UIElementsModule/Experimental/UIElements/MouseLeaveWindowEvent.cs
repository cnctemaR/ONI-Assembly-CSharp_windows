using System;

namespace UnityEngine.Experimental.UIElements
{
	public class MouseLeaveWindowEvent : MouseEventBase<MouseLeaveWindowEvent>
	{
		public MouseLeaveWindowEvent()
		{
			this.Init();
		}

		protected override void Init()
		{
			base.Init();
			base.flags = EventBase.EventFlags.Cancellable;
		}
	}
}
