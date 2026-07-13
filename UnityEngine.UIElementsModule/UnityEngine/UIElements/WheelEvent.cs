using System;

namespace UnityEngine.UIElements
{
	public class WheelEvent : MouseEventBase<WheelEvent>
	{
		static WheelEvent()
		{
			EventBase<WheelEvent>.SetCreateFunction(() => new WheelEvent());
		}

		public Vector3 delta { get; private set; }

		public new static WheelEvent GetPooled(Event systemEvent)
		{
			WheelEvent pooled = MouseEventBase<WheelEvent>.GetPooled(systemEvent);
			bool flag = systemEvent != null;
			if (flag)
			{
				pooled.delta = systemEvent.delta;
			}
			return pooled;
		}

		internal static WheelEvent GetPooled(Vector3 delta, Vector3 mousePosition, EventModifiers modifiers = EventModifiers.None)
		{
			WheelEvent pooled = EventBase<WheelEvent>.GetPooled();
			pooled.delta = delta;
			pooled.mousePosition = mousePosition;
			pooled.modifiers = modifiers;
			return pooled;
		}

		internal static WheelEvent GetPooled(Vector3 delta, IPointerEvent pointerEvent)
		{
			WheelEvent pooled = MouseEventBase<WheelEvent>.GetPooled(pointerEvent);
			pooled.delta = delta;
			return pooled;
		}

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		private void LocalInit()
		{
			base.propagation = EventBase.EventPropagation.Bubbles | EventBase.EventPropagation.TricklesDown | EventBase.EventPropagation.SkipDisabledElements;
			this.delta = Vector3.zero;
			base.recomputeTopElementUnderMouse = true;
		}

		public WheelEvent()
		{
			this.LocalInit();
		}

		internal override void Dispatch(BaseVisualElementPanel panel)
		{
			EventDispatchUtilities.DispatchToElementUnderPointerOrPanelRoot(this, panel, PointerId.mousePointerId, base.mousePosition);
		}

		public const float scrollDeltaPerTick = 3f;
	}
}
