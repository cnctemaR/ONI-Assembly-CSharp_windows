using System;

namespace UnityEngine.UIElements
{
	public sealed class TransitionEndEvent : TransitionEventBase<TransitionEndEvent>
	{
		static TransitionEndEvent()
		{
			EventBase<TransitionEndEvent>.SetCreateFunction(() => new TransitionEndEvent());
		}
	}
}
