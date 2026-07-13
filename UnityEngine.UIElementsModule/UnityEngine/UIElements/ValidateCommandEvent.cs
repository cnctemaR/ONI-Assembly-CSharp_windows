using System;

namespace UnityEngine.UIElements
{
	public class ValidateCommandEvent : CommandEventBase<ValidateCommandEvent>
	{
		static ValidateCommandEvent()
		{
			EventBase<ValidateCommandEvent>.SetCreateFunction(() => new ValidateCommandEvent());
		}
	}
}
