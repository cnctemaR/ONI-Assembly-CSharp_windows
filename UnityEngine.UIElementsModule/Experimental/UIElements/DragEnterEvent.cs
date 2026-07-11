using System;

namespace UnityEngine.Experimental.UIElements
{
	public class DragEnterEvent : DragAndDropEventBase<DragEnterEvent>
	{
		protected override void Init()
		{
			base.Init();
			base.flags = EventBase.EventFlags.TricklesDown;
		}
	}
}
