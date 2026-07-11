using System;

namespace UnityEngine.Experimental.UIElements
{
	public abstract class MouseCaptureEventBase<T> : EventBase<T>, IMouseCaptureEvent, IPropagatableEvent where T : MouseCaptureEventBase<T>, new()
	{
		protected MouseCaptureEventBase()
		{
			this.Init();
		}

		public IEventHandler relatedTarget { get; private set; }

		protected override void Init()
		{
			base.Init();
			base.flags = EventBase.EventFlags.Bubbles | EventBase.EventFlags.TricklesDown;
			this.relatedTarget = null;
		}

		public static T GetPooled(IEventHandler target, IEventHandler relatedTarget)
		{
			T pooled = EventBase<T>.GetPooled();
			pooled.target = target;
			pooled.relatedTarget = relatedTarget;
			return pooled;
		}
	}
}
