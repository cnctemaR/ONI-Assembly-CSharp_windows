using System;

namespace UnityEngine.UIElements
{
	public delegate void EventCallback<in TEventType>(TEventType evt);
}
