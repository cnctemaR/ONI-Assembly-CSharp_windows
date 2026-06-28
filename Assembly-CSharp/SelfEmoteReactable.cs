using System;
using UnityEngine;

public class SelfEmoteReactable : EmoteReactable
{
	public SelfEmoteReactable(GameObject gameObject, ChoreType chore_type, HashedString animset)
		: base(gameObject, chore_type, animset, 1, 1)
	{
	}

	public override bool InternalCanBegin(GameObject reactor)
	{
		if (reactor == null)
		{
			return false;
		}
		Navigator component = reactor.GetComponent<Navigator>();
		return !(component == null) && component.IsMoving() && this.reactionSource == reactor;
	}
}
