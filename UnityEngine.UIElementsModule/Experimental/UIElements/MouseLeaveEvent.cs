using System;

namespace UnityEngine.Experimental.UIElements
{
	public class MouseLeaveEvent : MouseEventBase<MouseLeaveEvent>
	{
		public MouseLeaveEvent()
		{
			this.Init();
		}

		protected override void Init()
		{
			base.Init();
			base.flags = EventBase.EventFlags.TricklesDown | EventBase.EventFlags.Cancellable;
		}
	}
}
