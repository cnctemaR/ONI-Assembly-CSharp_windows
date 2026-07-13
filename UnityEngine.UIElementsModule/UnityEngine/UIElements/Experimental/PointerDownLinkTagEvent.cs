using System;

namespace UnityEngine.UIElements.Experimental
{
	public sealed class PointerDownLinkTagEvent : PointerEventBase<PointerDownLinkTagEvent>
	{
		static PointerDownLinkTagEvent()
		{
			EventBase<PointerDownLinkTagEvent>.SetCreateFunction(() => new PointerDownLinkTagEvent());
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

		public static PointerDownLinkTagEvent GetPooled(IPointerEvent evt, string linkID, string linkText)
		{
			PointerDownLinkTagEvent pooled = PointerEventBase<PointerDownLinkTagEvent>.GetPooled(evt);
			pooled.linkID = linkID;
			pooled.linkText = linkText;
			return pooled;
		}

		public PointerDownLinkTagEvent()
		{
			this.LocalInit();
		}
	}
}
