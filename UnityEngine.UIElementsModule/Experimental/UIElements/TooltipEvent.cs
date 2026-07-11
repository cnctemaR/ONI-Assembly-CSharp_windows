using System;

namespace UnityEngine.Experimental.UIElements
{
	public class TooltipEvent : EventBase<TooltipEvent>, IPropagatableEvent
	{
		public TooltipEvent()
		{
			this.Init();
		}

		public string tooltip { get; set; }

		public Rect rect { get; set; }

		protected override void Init()
		{
			base.Init();
			base.flags = EventBase.EventFlags.Bubbles | EventBase.EventFlags.TricklesDown;
			this.rect = default(Rect);
			this.tooltip = string.Empty;
		}

		internal static TooltipEvent GetPooled(string tooltip, Rect rect)
		{
			TooltipEvent pooled = EventBase<TooltipEvent>.GetPooled();
			pooled.tooltip = tooltip;
			pooled.rect = rect;
			return pooled;
		}
	}
}
