using System;

namespace UnityEngine.UIElements
{
	public class ContextClickEvent : MouseEventBase<ContextClickEvent>
	{
		static ContextClickEvent()
		{
			EventBase<ContextClickEvent>.SetCreateFunction(() => new ContextClickEvent());
		}
	}
}
