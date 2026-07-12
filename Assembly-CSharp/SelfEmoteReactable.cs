using System;
using UnityEngine;

public class SelfEmoteReactable : EmoteReactable
{
	public SelfEmoteReactable(GameObject gameObject, HashedString id, ChoreType chore_type, float globalCooldown = 0f, float localCooldown = 20f, float lifeSpan = float.PositiveInfinity, float max_initial_delay = 0f)
		: base(gameObject, id, chore_type, 3, 3, globalCooldown, localCooldown, lifeSpan, max_initial_delay)
	{
	}

	public override bool InternalCanBegin(GameObject reactor, Navigator.ActiveTransition transition)
	{
		if (reactor != this.gameObject)
		{
			return false;
		}
		Navigator component = reactor.GetComponent<Navigator>();
		return !(component == null) && component.IsMoving();
	}

	public void PairEmote(EmoteChore emoteChore)
	{
		this.chore = emoteChore;
	}

	protected override void InternalEnd()
	{
		if (this.chore != null && this.chore.driver != null)
		{
			this.chore.PairReactable(null);
			this.chore.Cancel("Reactable ended");
			this.chore = null;
		}
		base.InternalEnd();
	}

	private EmoteChore chore;
}
