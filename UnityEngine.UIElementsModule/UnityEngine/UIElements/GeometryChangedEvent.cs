using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.Geometry)]
	public class GeometryChangedEvent : EventBase<GeometryChangedEvent>
	{
		static GeometryChangedEvent()
		{
			EventBase<GeometryChangedEvent>.SetCreateFunction(() => new GeometryChangedEvent());
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
			this.LocalInit();
		}

		private void LocalInit()
		{
			this.oldRect = Rect.zero;
			this.newRect = Rect.zero;
			this.layoutPass = 0;
		}

		public Rect oldRect { get; private set; }

		public Rect newRect { get; private set; }

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal int layoutPass { get; set; }

		public GeometryChangedEvent()
		{
			this.LocalInit();
		}
	}
}
