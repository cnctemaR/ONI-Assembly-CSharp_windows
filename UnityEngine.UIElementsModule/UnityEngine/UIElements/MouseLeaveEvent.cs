using System;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.EnterLeave)]
	public class MouseLeaveEvent : MouseEventBase<MouseLeaveEvent>
	{
		static MouseLeaveEvent()
		{
			EventBase<MouseLeaveEvent>.SetCreateFunction(() => new MouseLeaveEvent());
		}

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		private void LocalInit()
		{
			base.propagation = EventBase.EventPropagation.TricklesDown;
		}

		public MouseLeaveEvent()
		{
			this.LocalInit();
		}

		internal override void Dispatch(BaseVisualElementPanel panel)
		{
			EventDispatchUtilities.DispatchToAssignedTarget(this, panel);
		}
	}
}
