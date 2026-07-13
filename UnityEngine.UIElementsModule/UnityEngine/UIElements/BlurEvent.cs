using System;

namespace UnityEngine.UIElements
{
	public class BlurEvent : FocusEventBase<BlurEvent>
	{
		static BlurEvent()
		{
			EventBase<BlurEvent>.SetCreateFunction(() => new BlurEvent());
		}
	}
}
