using System;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.PointerMove)]
	public class MouseMoveEvent : MouseEventBase<MouseMoveEvent>
	{
		static MouseMoveEvent()
		{
			EventBase<MouseMoveEvent>.SetCreateFunction(() => new MouseMoveEvent());
		}

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		private void LocalInit()
		{
			base.propagation = EventBase.EventPropagation.BubblesOrTricklesDown;
			base.recomputeTopElementUnderMouse = true;
		}

		public MouseMoveEvent()
		{
			this.LocalInit();
		}

		public new static MouseMoveEvent GetPooled(Event systemEvent)
		{
			MouseMoveEvent pooled = MouseEventBase<MouseMoveEvent>.GetPooled(systemEvent);
			pooled.button = 0;
			return pooled;
		}

		internal static MouseMoveEvent GetPooled(PointerMoveEvent pointerEvent)
		{
			return MouseEventBase<MouseMoveEvent>.GetPooled(pointerEvent);
		}
	}
}
