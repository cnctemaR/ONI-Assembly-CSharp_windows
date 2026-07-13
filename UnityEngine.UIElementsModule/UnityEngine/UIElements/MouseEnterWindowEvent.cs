using System;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.EnterLeaveWindow)]
	public class MouseEnterWindowEvent : MouseEventBase<MouseEnterWindowEvent>
	{
		static MouseEnterWindowEvent()
		{
			EventBase<MouseEnterWindowEvent>.SetCreateFunction(() => new MouseEnterWindowEvent());
		}

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		private void LocalInit()
		{
			base.propagation = EventBase.EventPropagation.Bubbles;
			base.recomputeTopElementUnderMouse = true;
		}

		public MouseEnterWindowEvent()
		{
			this.LocalInit();
		}

		internal override void Dispatch(BaseVisualElementPanel panel)
		{
			EventDispatchUtilities.DispatchToElementUnderPointerOrPanelRoot(this, panel, PointerId.mousePointerId, base.mousePosition);
		}
	}
}
