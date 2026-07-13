using System;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.PointerDown)]
	public class MouseDownEvent : MouseEventBase<MouseDownEvent>
	{
		static MouseDownEvent()
		{
			EventBase<MouseDownEvent>.SetCreateFunction(() => new MouseDownEvent());
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

		public MouseDownEvent()
		{
			this.LocalInit();
		}

		public new static MouseDownEvent GetPooled(Event systemEvent)
		{
			return MouseEventBase<MouseDownEvent>.GetPooled(systemEvent);
		}

		private static MouseDownEvent MakeFromPointerEvent(IPointerEvent pointerEvent)
		{
			return MouseEventBase<MouseDownEvent>.GetPooled(pointerEvent);
		}

		internal static MouseDownEvent GetPooled(PointerDownEvent pointerEvent)
		{
			return MouseDownEvent.MakeFromPointerEvent(pointerEvent);
		}

		internal static MouseDownEvent GetPooled(PointerMoveEvent pointerEvent)
		{
			return MouseDownEvent.MakeFromPointerEvent(pointerEvent);
		}
	}
}
