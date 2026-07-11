using System;

namespace UnityEngine.Experimental.UIElements
{
	public class FocusInEvent : FocusEventBase<FocusInEvent>
	{
		public FocusInEvent()
		{
			this.Init();
		}

		protected override void Init()
		{
			base.Init();
			base.flags = EventBase.EventFlags.Bubbles | EventBase.EventFlags.TricklesDown;
		}
	}
}
