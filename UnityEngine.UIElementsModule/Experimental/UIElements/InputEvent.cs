using System;

namespace UnityEngine.Experimental.UIElements
{
	public class InputEvent : EventBase<InputEvent>
	{
		public InputEvent()
		{
			this.Init();
		}

		public string previousData { get; protected set; }

		public string newData { get; protected set; }

		protected override void Init()
		{
			base.Init();
			base.flags = EventBase.EventFlags.Bubbles | EventBase.EventFlags.TricklesDown;
			this.previousData = null;
			this.newData = null;
		}

		public static InputEvent GetPooled(string previousData, string newData)
		{
			InputEvent pooled = EventBase<InputEvent>.GetPooled();
			pooled.previousData = previousData;
			pooled.newData = newData;
			return pooled;
		}
	}
}
