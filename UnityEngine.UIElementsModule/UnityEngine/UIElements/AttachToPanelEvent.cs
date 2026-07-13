using System;

namespace UnityEngine.UIElements
{
	public class AttachToPanelEvent : PanelChangedEventBase<AttachToPanelEvent>
	{
		static AttachToPanelEvent()
		{
			EventBase<AttachToPanelEvent>.SetCreateFunction(() => new AttachToPanelEvent());
		}
	}
}
