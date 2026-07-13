using System;

namespace UnityEngine.UIElements
{
	public class MouseCaptureEvent : MouseCaptureEventBase<MouseCaptureEvent>
	{
		static MouseCaptureEvent()
		{
			EventBase<MouseCaptureEvent>.SetCreateFunction(() => new MouseCaptureEvent());
		}
	}
}
