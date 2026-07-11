using System;

namespace UnityEngine.Experimental.UIElements
{
	public abstract class FocusEventBase<T> : EventBase<T>, IFocusEvent, IPropagatableEvent where T : FocusEventBase<T>, new()
	{
		protected FocusEventBase()
		{
			this.Init();
		}

		public Focusable relatedTarget { get; protected set; }

		public FocusChangeDirection direction { get; protected set; }

		protected override void Init()
		{
			base.Init();
			base.flags = EventBase.EventFlags.TricklesDown;
			this.relatedTarget = null;
			this.direction = FocusChangeDirection.unspecified;
			this.m_FocusController = null;
		}

		public static T GetPooled(IEventHandler target, Focusable relatedTarget, FocusChangeDirection direction, FocusController focusController)
		{
			T pooled = EventBase<T>.GetPooled();
			pooled.target = target;
			pooled.relatedTarget = relatedTarget;
			pooled.direction = direction;
			pooled.m_FocusController = focusController;
			return pooled;
		}

		protected FocusController m_FocusController;
	}
}
