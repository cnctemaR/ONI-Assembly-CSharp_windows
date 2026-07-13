using System;

namespace UnityEngine.UIElements
{
	public sealed class TransitionStartEvent : TransitionEventBase<TransitionStartEvent>
	{
		static TransitionStartEvent()
		{
			EventBase<TransitionStartEvent>.SetCreateFunction(() => new TransitionStartEvent());
		}
	}
}
