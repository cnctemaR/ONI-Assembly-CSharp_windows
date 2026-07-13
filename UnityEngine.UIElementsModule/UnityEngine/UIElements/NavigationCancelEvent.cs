using System;

namespace UnityEngine.UIElements
{
	public class NavigationCancelEvent : NavigationEventBase<NavigationCancelEvent>
	{
		static NavigationCancelEvent()
		{
			EventBase<NavigationCancelEvent>.SetCreateFunction(() => new NavigationCancelEvent());
		}
	}
}
