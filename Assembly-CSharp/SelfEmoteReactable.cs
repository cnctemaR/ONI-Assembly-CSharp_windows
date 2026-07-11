using System;
using UnityEngine;

public class SelfEmoteReactable : EmoteReactable
{
	public SelfEmoteReactable(GameObject gameObject, HashedString id, ChoreType chore_type, HashedString animset, float min_reactable_time = 0f, float min_reactor_time = 0f, float max_trigger_time = float.PositiveInfinity)
		: base(gameObject, id, chore_type, animset, 3, 3, min_reactable_time, min_reactor_time, max_trigger_time)
	{
	}

	public override bool InternalCanBegin(GameObject reactor, Navigator.ActiveTransition transition)
	{
		if (reactor == null)
		{
			return false;
		}
		Navigator component = reactor.GetComponent<Navigator>();
		return !(component == null) && component.IsMoving() && this.gameObject == reactor;
	}

	public void PairEmote(EmoteChore emote)
	{
		this.emote = emote;
	}

	protected override void InternalEnd()
	{
		if (this.emote != null && this.emote.driver != null)
		{
			this.emote.PairReactable(null);
			this.emote.Cancel("Reactable ended");
			this.emote = null;
		}
		base.InternalEnd();
	}

	private EmoteChore emote;
}
