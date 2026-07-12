using System;

public class KButtonEvent : KInputEvent
{
	public KButtonEvent(KInputController controller, InputEventType event_type, bool[] is_action)
		: base(controller, event_type)
	{
		this.mIsAction = is_action;
	}

	public KButtonEvent(KInputController controller, InputEventType event_type, global::Action action)
		: base(controller, event_type)
	{
		this.mIsAction = null;
		this.mAction = action;
	}

	public bool TryConsume(global::Action action)
	{
		if (base.Consumed)
		{
			Debug.LogError(action.ToString() + " was already consumed");
		}
		if (action != global::Action.NumActions)
		{
			if (this.mIsAction != null)
			{
				if (this.mIsAction[(int)action])
				{
					base.Consumed = true;
				}
			}
			else if (this.mAction == action)
			{
				base.Consumed = true;
			}
		}
		return base.Consumed;
	}

	public bool IsAction(global::Action action)
	{
		if (this.mIsAction != null)
		{
			return this.mIsAction[(int)action];
		}
		return this.mAction == action;
	}

	public global::Action GetAction()
	{
		if (this.mIsAction != null)
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
		return this.mAction;
	}

	private bool[] mIsAction;

	private global::Action mAction;
}
