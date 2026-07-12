using System;

namespace UnityEngine.UIElements
{
	public class PointerCaptureEvent : PointerCaptureEventBase<PointerCaptureEvent>
	{
		static PointerCaptureEvent()
		{
			EventBase<PointerCaptureEvent>.SetCreateFunction(() => new PointerCaptureEvent());
		}
	}
}
