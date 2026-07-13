using System;

namespace UnityEngine.UIElements
{
	public class MouseUpEvent : MouseEventBase<MouseUpEvent>
	{
		static MouseUpEvent()
		{
			EventBase<MouseUpEvent>.SetCreateFunction(() => new MouseUpEvent());
		}

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		private void LocalInit()
		{
			base.propagation = EventBase.EventPropagation.Bubbles | EventBase.EventPropagation.TricklesDown | EventBase.EventPropagation.SkipDisabledElements;
			base.recomputeTopElementUnderMouse = true;
		}

		public MouseUpEvent()
		{
			this.LocalInit();
		}

		public new static MouseUpEvent GetPooled(Event systemEvent)
		{
			return MouseEventBase<MouseUpEvent>.GetPooled(systemEvent);
		}

		private static MouseUpEvent MakeFromPointerEvent(IPointerEvent pointerEvent)
		{
			return MouseEventBase<MouseUpEvent>.GetPooled(pointerEvent);
		}

		internal static MouseUpEvent GetPooled(PointerUpEvent pointerEvent)
		{
			return MouseUpEvent.MakeFromPointerEvent(pointerEvent);
		}

		internal static MouseUpEvent GetPooled(PointerMoveEvent pointerEvent)
		{
			return MouseUpEvent.MakeFromPointerEvent(pointerEvent);
		}

		internal static MouseUpEvent GetPooled(PointerCancelEvent pointerEvent)
		{
			return MouseUpEvent.MakeFromPointerEvent(pointerEvent);
		}
	}
}
