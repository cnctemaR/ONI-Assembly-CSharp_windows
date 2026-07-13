using System;

namespace UnityEngine.UIElements.Experimental
{
	[EventCategory(EventCategory.PointerMove)]
	public class PointerMoveLinkTagEvent : PointerEventBase<PointerMoveLinkTagEvent>
	{
		static PointerMoveLinkTagEvent()
		{
			EventBase<PointerMoveLinkTagEvent>.SetCreateFunction(() => new PointerMoveLinkTagEvent());
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

		public static PointerMoveLinkTagEvent GetPooled(IPointerEvent evt, string linkID, string linkText)
		{
			PointerMoveLinkTagEvent pooled = PointerEventBase<PointerMoveLinkTagEvent>.GetPooled(evt);
			pooled.linkID = linkID;
			pooled.linkText = linkText;
			return pooled;
		}

		public PointerMoveLinkTagEvent()
		{
			this.LocalInit();
		}
	}
}
