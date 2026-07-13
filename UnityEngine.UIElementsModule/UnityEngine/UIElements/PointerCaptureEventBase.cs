using System;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.Pointer)]
	public abstract class PointerCaptureEventBase<T> : EventBase<T>, IPointerCaptureEvent, IPointerCaptureEventInternal where T : PointerCaptureEventBase<T>, new()
	{
		public IEventHandler relatedTarget { get; private set; }

		public int pointerId { get; private set; }

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		private void LocalInit()
		{
			base.propagation = EventBase.EventPropagation.BubblesOrTricklesDown;
			this.relatedTarget = null;
			this.pointerId = PointerId.invalidPointerId;
		}

		public static T GetPooled(IEventHandler target, IEventHandler relatedTarget, int pointerId)
		{
			T pooled = EventBase<T>.GetPooled();
			pooled.elementTarget = (VisualElement)target;
			pooled.relatedTarget = relatedTarget;
			pooled.pointerId = pointerId;
			return pooled;
		}

		protected PointerCaptureEventBase()
		{
			this.LocalInit();
		}
	}
}
