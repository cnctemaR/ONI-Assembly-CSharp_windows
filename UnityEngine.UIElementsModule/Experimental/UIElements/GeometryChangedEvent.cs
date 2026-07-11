using System;

namespace UnityEngine.Experimental.UIElements
{
	public class GeometryChangedEvent : EventBase<GeometryChangedEvent>, IPropagatableEvent
	{
		public GeometryChangedEvent()
		{
			this.Init();
		}

		public static GeometryChangedEvent GetPooled(Rect oldRect, Rect newRect)
		{
			GeometryChangedEvent pooled = EventBase<GeometryChangedEvent>.GetPooled();
			pooled.oldRect = oldRect;
			pooled.newRect = newRect;
			return pooled;
		}

		protected override void Init()
		{
			base.Init();
			this.oldRect = Rect.zero;
			this.newRect = Rect.zero;
		}

		public Rect oldRect { get; private set; }

		public Rect newRect { get; private set; }
	}
}
