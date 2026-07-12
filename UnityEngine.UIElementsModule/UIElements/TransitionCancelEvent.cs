using System;

namespace UnityEngine.UIElements
{
	public sealed class TransitionCancelEvent : TransitionEventBase<TransitionCancelEvent>
	{
		static TransitionCancelEvent()
		{
			EventBase<TransitionCancelEvent>.SetCreateFunction(() => new TransitionCancelEvent());
		}
	}
}
