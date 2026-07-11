using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

public class DupeGreetingManager : KMonoBehaviour, ISim200ms
{
	protected override void OnPrefabInit()
	{
		this.candidateCells = new Dictionary<int, MinionIdentity>();
		this.activeSetups = new List<DupeGreetingManager.GreetingSetup>();
		this.cooldowns = new Dictionary<MinionIdentity, float>();
	}

	public void Sim200ms(float dt)
	{
		if (GameClock.Instance.GetTime() / 600f < TuningData<DupeGreetingManager.Tuning>.Get().cyclesBeforeFirstGreeting)
		{
			return;
		}
		for (int i = this.activeSetups.Count - 1; i >= 0; i--)
		{
			DupeGreetingManager.GreetingSetup greetingSetup = this.activeSetups[i];
			if (!this.ValidNavigatingMinion(greetingSetup.A.minion) || !this.ValidOppositionalMinion(greetingSetup.A.minion, greetingSetup.B.minion))
			{
				greetingSetup.A.reactable.Cleanup();
				greetingSetup.B.reactable.Cleanup();
				this.activeSetups.RemoveAt(i);
			}
		}
		this.candidateCells.Clear();
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
		{
			if ((!this.cooldowns.ContainsKey(minionIdentity) || GameClock.Instance.GetTime() - this.cooldowns[minionIdentity] >= 720f * TuningData<DupeGreetingManager.Tuning>.Get().greetingDelayMultiplier) && this.ValidNavigatingMinion(minionIdentity))
			{
				for (int j = 0; j <= 2; j++)
				{
					int offsetCell = this.GetOffsetCell(minionIdentity, j);
					if (this.candidateCells.ContainsKey(offsetCell) && this.ValidOppositionalMinion(minionIdentity, this.candidateCells[offsetCell]))
					{
						this.BeginNewGreeting(minionIdentity, this.candidateCells[offsetCell], offsetCell);
						break;
					}
					this.candidateCells[offsetCell] = minionIdentity;
				}
			}
		}
	}

	private int GetOffsetCell(MinionIdentity minion, int offset)
	{
		if (!minion.GetComponent<Facing>().GetFacing())
		{
			return Grid.OffsetCell(Grid.PosToCell(minion), offset, 0);
		}
		return Grid.OffsetCell(Grid.PosToCell(minion), -offset, 0);
	}

	private bool ValidNavigatingMinion(MinionIdentity minion)
	{
		if (minion == null)
		{
			return false;
		}
		Navigator component = minion.GetComponent<Navigator>();
		return component != null && component.IsMoving() && component.CurrentNavType == NavType.Floor;
	}

	private bool ValidOppositionalMinion(MinionIdentity reference_minion, MinionIdentity minion)
	{
		if (reference_minion == null)
		{
			return false;
		}
		if (minion == null)
		{
			return false;
		}
		Facing component = minion.GetComponent<Facing>();
		Facing component2 = reference_minion.GetComponent<Facing>();
		return this.ValidNavigatingMinion(minion) && component != null && component2 != null && component.GetFacing() != component2.GetFacing();
	}

	private void BeginNewGreeting(MinionIdentity minion_a, MinionIdentity minion_b, int cell)
	{
		DupeGreetingManager.GreetingSetup greetingSetup = new DupeGreetingManager.GreetingSetup();
		greetingSetup.cell = cell;
		greetingSetup.A = new DupeGreetingManager.GreetingUnit(minion_a, this.GetReactable(minion_a));
		greetingSetup.B = new DupeGreetingManager.GreetingUnit(minion_b, this.GetReactable(minion_b));
		this.activeSetups.Add(greetingSetup);
	}

	private Reactable GetReactable(MinionIdentity minion)
	{
		return new SelfEmoteReactable(minion.gameObject, "NavigatorPassingGreeting", Db.Get().ChoreTypes.Emote, DupeGreetingManager.waveAnims[global::UnityEngine.Random.Range(0, DupeGreetingManager.waveAnims.Count)], 1000f, 20f, float.PositiveInfinity).AddStep(new EmoteReactable.EmoteStep
		{
			anim = "react",
			startcb = new Action<GameObject>(this.BeginReacting)
		}).AddThought(Db.Get().Thoughts.Chatty);
	}

	private void BeginReacting(GameObject minionGO)
	{
		MinionIdentity component = minionGO.GetComponent<MinionIdentity>();
		Vector3 vector = Vector3.zero;
		foreach (DupeGreetingManager.GreetingSetup greetingSetup in this.activeSetups)
		{
			if (greetingSetup.A.minion == component)
			{
				vector = greetingSetup.B.minion.transform.GetPosition();
				break;
			}
			if (greetingSetup.B.minion == component)
			{
				vector = greetingSetup.A.minion.transform.GetPosition();
				break;
			}
		}
		minionGO.GetComponent<Facing>().SetFacing(vector.x < minionGO.transform.GetPosition().x);
		minionGO.GetComponent<Effects>().Add("Greeting", true);
		this.cooldowns[component] = GameClock.Instance.GetTime();
	}

	private const float COOLDOWN_TIME = 720f;

	private Dictionary<int, MinionIdentity> candidateCells;

	private List<DupeGreetingManager.GreetingSetup> activeSetups;

	private Dictionary<MinionIdentity, float> cooldowns;

	private static readonly List<string> waveAnims = new List<string> { "anim_react_wave_kanim", "anim_react_wave_shy_kanim", "anim_react_fingerguns_kanim" };

	public class Tuning : TuningData<DupeGreetingManager.Tuning>
	{
		public float cyclesBeforeFirstGreeting;

		public float greetingDelayMultiplier;
	}

	private class GreetingUnit
	{
		public GreetingUnit(MinionIdentity minion, Reactable reactable)
		{
			this.minion = minion;
			this.reactable = reactable;
		}

		public MinionIdentity minion;

		public Reactable reactable;
	}

	private class GreetingSetup
	{
		public int cell;

		public DupeGreetingManager.GreetingUnit A;

		public DupeGreetingManager.GreetingUnit B;
	}
}
