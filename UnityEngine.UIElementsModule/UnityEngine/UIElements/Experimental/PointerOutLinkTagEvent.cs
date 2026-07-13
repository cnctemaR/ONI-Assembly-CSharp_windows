using System;

namespace UnityEngine.UIElements.Experimental
{
	[EventCategory(EventCategory.EnterLeave)]
	public class PointerOutLinkTagEvent : PointerEventBase<PointerOutLinkTagEvent>
	{
		static PointerOutLinkTagEvent()
		{
			EventBase<PointerOutLinkTagEvent>.SetCreateFunction(() => new PointerOutLinkTagEvent());
		}

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		private void LocalInit()
		{
			base.propagation = EventBase.EventPropagation.BubblesOrTricklesDown;
		}

		public static PointerOutLinkTagEvent GetPooled(IPointerEvent evt, string linkID)
		{
			return PointerEventBase<PointerOutLinkTagEvent>.GetPooled(evt);
		}

		public PointerOutLinkTagEvent()
		{
			this.LocalInit();
		}
	}
}
