using System;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.EnterLeave)]
	public class MouseEnterEvent : MouseEventBase<MouseEnterEvent>
	{
		static MouseEnterEvent()
		{
			EventBase<MouseEnterEvent>.SetCreateFunction(() => new MouseEnterEvent());
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

		public MouseEnterEvent()
		{
			this.LocalInit();
		}

		internal override void Dispatch(BaseVisualElementPanel panel)
		{
			EventDispatchUtilities.DispatchToAssignedTarget(this, panel);
		}
	}
}
