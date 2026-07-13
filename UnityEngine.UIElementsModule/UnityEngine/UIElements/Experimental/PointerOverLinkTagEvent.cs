using System;

namespace UnityEngine.UIElements.Experimental
{
	[EventCategory(EventCategory.EnterLeave)]
	public class PointerOverLinkTagEvent : PointerEventBase<PointerOverLinkTagEvent>
	{
		static PointerOverLinkTagEvent()
		{
			EventBase<PointerOverLinkTagEvent>.SetCreateFunction(() => new PointerOverLinkTagEvent());
		}

		public string linkID { get; private set; }

		public string linkText { get; private set; }

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		private void LocalInit()
		{
			base.propagation = EventBase.EventPropagation.BubblesOrTricklesDown;
		}

		public static PointerOverLinkTagEvent GetPooled(IPointerEvent evt, string linkID, string linkText)
		{
			PointerOverLinkTagEvent pooled = PointerEventBase<PointerOverLinkTagEvent>.GetPooled(evt);
			pooled.linkID = linkID;
			pooled.linkText = linkText;
			return pooled;
		}

		public PointerOverLinkTagEvent()
		{
			this.LocalInit();
		}
	}
}
