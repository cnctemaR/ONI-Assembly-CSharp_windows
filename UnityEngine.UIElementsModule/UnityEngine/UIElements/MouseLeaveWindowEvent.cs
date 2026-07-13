using System;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.EnterLeaveWindow)]
	public class MouseLeaveWindowEvent : MouseEventBase<MouseLeaveWindowEvent>
	{
		static MouseLeaveWindowEvent()
		{
			EventBase<MouseLeaveWindowEvent>.SetCreateFunction(() => new MouseLeaveWindowEvent());
		}

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		private void LocalInit()
		{
			base.propagation = EventBase.EventPropagation.Bubbles;
			base.recomputeTopElementUnderMouse = false;
		}

		public MouseLeaveWindowEvent()
		{
			this.LocalInit();
		}

		public new static MouseLeaveWindowEvent GetPooled(Event systemEvent)
		{
			bool flag = systemEvent != null;
			if (flag)
			{
				PointerDeviceState.ReleaseAllButtons(PointerId.mousePointerId);
			}
			return MouseEventBase<MouseLeaveWindowEvent>.GetPooled(systemEvent);
		}

		protected internal override void PostDispatch(IPanel panel)
		{
			BaseVisualElementPanel baseVisualElementPanel;
			bool flag;
			if (base.pressedButtons == 0)
			{
				baseVisualElementPanel = panel as BaseVisualElementPanel;
				flag = baseVisualElementPanel != null;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				baseVisualElementPanel.ClearCachedElementUnderPointer(PointerId.mousePointerId, this);
				baseVisualElementPanel.CommitElementUnderPointers();
			}
			base.PostDispatch(panel);
		}

		internal override void Dispatch(BaseVisualElementPanel panel)
		{
			EventDispatchUtilities.DispatchToElementUnderPointerOrPanelRoot(this, panel, PointerId.mousePointerId, base.mousePosition);
		}
	}
}
