using System;

namespace UnityEngine.UIElements
{
	public class KeyUpEvent : KeyboardEventBase<KeyUpEvent>
	{
		static KeyUpEvent()
		{
			EventBase<KeyUpEvent>.SetCreateFunction(() => new KeyUpEvent());
		}
	}
}
