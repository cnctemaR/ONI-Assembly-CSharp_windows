using System;

namespace UnityEngine.Experimental.UIElements
{
	public class MouseEnterWindowEvent : MouseEventBase<MouseEnterWindowEvent>
	{
		public MouseEnterWindowEvent()
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
