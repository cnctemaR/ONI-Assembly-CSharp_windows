using System;

public class KButtonEvent : KInputEvent
{
	public KButtonEvent(KInputController controller, InputEventType event_type, bool[] is_action)
		: base(controller, event_type)
	{
		this.mIsAction = is_action;
	}

	public bool TryConsume(global::Action action)
	{
		DebugUtil.Assert(!base.Consumed, action.ToString() + " was already consumed");
		if (action != global::Action.NumActions)
		{
			if (this.mIsAction[(int)action])
			{
				base.Consumed = true;
			}
		}
		return base.Consumed;
	}

	public bool IsAction(global::Action action)
	{
		return this.mIsAction[(int)action];
	}

	public global::Action GetAction()
	{
		for (int i = 0; i < this.mIsAction.Length; i++)
		{
			if (this.mIsAction[i])
			{
				return (global::Action)i;
			}
		}
		return global::Action.NumActions;
	}

	private bool[] mIsAction;
}
