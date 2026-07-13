using System;

namespace UnityEngine.UIElements.Experimental
{
	public class PointerUpLinkTagEvent : PointerEventBase<PointerUpLinkTagEvent>
	{
		static PointerUpLinkTagEvent()
		{
			EventBase<PointerUpLinkTagEvent>.SetCreateFunction(() => new PointerUpLinkTagEvent());
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

		public static PointerUpLinkTagEvent GetPooled(IPointerEvent evt, string linkID, string linkText)
		{
			PointerUpLinkTagEvent pooled = PointerEventBase<PointerUpLinkTagEvent>.GetPooled(evt);
			pooled.linkID = linkID;
			pooled.linkText = linkText;
			return pooled;
		}

		public PointerUpLinkTagEvent()
		{
			this.LocalInit();
		}
	}
}
