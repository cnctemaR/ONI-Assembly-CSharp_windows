using System;

namespace UnityEngine.UIElements
{
	public sealed class TransitionRunEvent : TransitionEventBase<TransitionRunEvent>
	{
		static TransitionRunEvent()
		{
			EventBase<TransitionRunEvent>.SetCreateFunction(() => new TransitionRunEvent());
		}
	}
}
