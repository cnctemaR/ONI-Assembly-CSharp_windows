using System;

namespace UnityEngine.UIElements
{
	public class NavigationSubmitEvent : NavigationEventBase<NavigationSubmitEvent>
	{
		static NavigationSubmitEvent()
		{
			EventBase<NavigationSubmitEvent>.SetCreateFunction(() => new NavigationSubmitEvent());
		}
	}
}
