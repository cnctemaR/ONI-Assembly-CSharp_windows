using System;

namespace UnityEngine.UIElements
{
	public class ExecuteCommandEvent : CommandEventBase<ExecuteCommandEvent>
	{
		static ExecuteCommandEvent()
		{
			EventBase<ExecuteCommandEvent>.SetCreateFunction(() => new ExecuteCommandEvent());
		}
	}
}
