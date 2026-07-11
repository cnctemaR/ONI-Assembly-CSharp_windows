using System;

namespace UnityEngine.Experimental.UIElements
{
	public class DragLeaveEvent : DragAndDropEventBase<DragLeaveEvent>
	{
		public DragLeaveEvent()
		{
			this.Init();
		}

		protected override void Init()
		{
			base.Init();
			base.flags = EventBase.EventFlags.TricklesDown;
		}
	}
}
