using System;
using UnityEngine;

public class SelfEmoteReactable : EmoteReactable
{
	public SelfEmoteReactable(GameObject gameObject, ChoreType chore_type, HashedString animset)
		: base(gameObject, chore_type, animset, 1, 1)
	{
	}

	public override bool InternalCanBegin(GameObject reactor, Navigator.ActiveTransition transition)
	{
		bool flag;
		if (reactor == null)
		{
			flag = false;
		}
		else
		{
			Navigator component = reactor.GetComponent<Navigator>();
			flag = !(component == null) && component.IsMoving() && this.reactionSource == reactor;
		}
		return flag;
	}
}
