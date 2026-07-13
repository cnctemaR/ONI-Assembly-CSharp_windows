using System;

namespace UnityEngine.UIElements
{
	public sealed class ClickEvent : PointerEventBase<ClickEvent>
	{
		static ClickEvent()
		{
			EventBase<ClickEvent>.SetCreateFunction(() => new ClickEvent());
		}

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		private void LocalInit()
		{
			base.propagation = EventBase.EventPropagation.Bubbles | EventBase.EventPropagation.TricklesDown | EventBase.EventPropagation.SkipDisabledElements;
		}

		public ClickEvent()
		{
			this.LocalInit();
		}

		internal static ClickEvent GetPooled(IPointerEvent pointerEvent, int clickCount)
		{
			ClickEvent pooled = PointerEventBase<ClickEvent>.GetPooled(pointerEvent);
			pooled.clickCount = clickCount;
			return pooled;
		}
	}
}
