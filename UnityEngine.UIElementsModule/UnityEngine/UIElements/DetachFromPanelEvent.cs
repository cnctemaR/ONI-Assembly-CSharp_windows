using System;

namespace UnityEngine.UIElements
{
	public class DetachFromPanelEvent : PanelChangedEventBase<DetachFromPanelEvent>
	{
		static DetachFromPanelEvent()
		{
			EventBase<DetachFromPanelEvent>.SetCreateFunction(() => new DetachFromPanelEvent());
		}
	}
}
