using System;

namespace UnityEngine.Experimental.UIElements
{
	public class DragExitedEvent : DragAndDropEventBase<DragExitedEvent>
	{
		protected override void Init()
		{
			base.Init();
			base.flags = EventBase.EventFlags.Bubbles | EventBase.EventFlags.TricklesDown;
		}
	}
}
