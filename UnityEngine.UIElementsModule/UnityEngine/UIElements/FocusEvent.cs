using System;

namespace UnityEngine.UIElements
{
	public class FocusEvent : FocusEventBase<FocusEvent>
	{
		static FocusEvent()
		{
			EventBase<FocusEvent>.SetCreateFunction(() => new FocusEvent());
		}
	}
}
