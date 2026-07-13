using System;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.Keyboard)]
	internal class IMEEvent : EventBase<IMEEvent>
	{
		public string compositionString { get; protected set; }

		static IMEEvent()
		{
			EventBase<IMEEvent>.SetCreateFunction(() => new IMEEvent());
		}

		public static IMEEvent GetPooled(string compositionString)
		{
			IMEEvent pooled = EventBase<IMEEvent>.GetPooled();
			pooled.compositionString = compositionString;
			return pooled;
		}

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		private void LocalInit()
		{
			base.propagation = EventBase.EventPropagation.Bubbles | EventBase.EventPropagation.TricklesDown | EventBase.EventPropagation.SkipDisabledElements;
			this.compositionString = null;
		}

		public IMEEvent()
		{
			this.LocalInit();
		}
	}
}
