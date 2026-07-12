using System;

namespace UnityEngine.UIElements
{
	public class FocusOutEvent : FocusEventBase<FocusOutEvent>
	{
		static FocusOutEvent()
		{
			EventBase<FocusOutEvent>.SetCreateFunction(() => new FocusOutEvent());
		}

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		private void LocalInit()
		{
			base.propagation = EventBase.EventPropagation.Bubbles | EventBase.EventPropagation.TricklesDown;
		}

		public FocusOutEvent()
		{
			this.LocalInit();
		}
	}
}
