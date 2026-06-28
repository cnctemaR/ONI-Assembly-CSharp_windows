using System;
using Klei.AI;
using TUNING;
using UnityEngine;

[SkipSerialization]
public class Narcolepsy : StateMachineComponent<Narcolepsy.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		this.Subscribe(1623392196, new EventSystem.EventHandler(this.OnDeath));
		this.Subscribe(-1117766961, new EventSystem.EventHandler(this.OnRevived));
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	private void OnDeath(object data)
	{
		base.enabled = false;
	}

	private void OnRevived(object data)
	{
		base.enabled = true;
	}

	public bool IsNarcolepsing()
	{
		return base.smi.IsNarcolepsing();
	}

	public void ModifyTrait(Trait t)
	{
	}

	public class StatesInstance : GameStateMachine<Narcolepsy.States, Narcolepsy.StatesInstance, Narcolepsy>.GameInstance
	{
		public StatesInstance(Narcolepsy master)
			: base(master)
		{
		}

		public bool IsSleeping()
		{
			StaminaMonitor.Instance smi = base.master.GetSMI<StaminaMonitor.Instance>();
			return smi != null && smi.IsSleeping();
		}

		public bool IsNarcolepsing()
		{
			return this.GetCurrentState() == base.sm.sleepy;
		}
	}

	public class States : GameStateMachine<Narcolepsy.States, Narcolepsy.StatesInstance, Narcolepsy>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.root.EventTransition(GameHashes.Died, this.dead, (Narcolepsy.StatesInstance smi) => smi.Get<Health>() != null && smi.Get<Health>().IsDead());
			this.idle.Enter("ScheduleNextSleep", delegate(Narcolepsy.StatesInstance smi)
			{
				smi.ScheduleGoTo(this.GetNewInterval(TRAITS.NARCOLEPSY_INTERVAL_MIN, TRAITS.NARCOLEPSY_INTERVAL_MAX), this.sleepy);
			});
			this.sleepy.Enter("Is Already Sleeping Check", delegate(Narcolepsy.StatesInstance smi)
			{
				if (smi.master.GetSMI<StaminaMonitor.Instance>().IsSleeping())
				{
					smi.GoTo(this.idle);
				}
				else
				{
					smi.ScheduleGoTo(this.GetNewInterval(TRAITS.NARCOLEPSY_SLEEPDURATION_MIN, TRAITS.NARCOLEPSY_SLEEPDURATION_MAX), this.idle);
				}
			}).ToggleUrge(Db.Get().Urges.Sleep).ToggleChore(new Func<Narcolepsy.StatesInstance, Chore>(this.CreateSleepOnFloorChore), this.idle, false);
			this.dead.DoNothing();
		}

		private Chore CreateSleepOnFloorChore(Narcolepsy.StatesInstance smi)
		{
			return new SleepOnFloorChore(smi.master);
		}

		private float GetNewInterval(float min, float max)
		{
			float num = max - min;
			float num2 = Util.GaussianRandom(num, 1f);
			num2 = Mathf.Max(num2, min);
			num2 = Mathf.Min(num2, max);
			return global::UnityEngine.Random.Range(min, max);
		}

		public GameStateMachine<Narcolepsy.States, Narcolepsy.StatesInstance, Narcolepsy>.State idle;

		public GameStateMachine<Narcolepsy.States, Narcolepsy.StatesInstance, Narcolepsy>.State sleepy;

		public GameStateMachine<Narcolepsy.States, Narcolepsy.StatesInstance, Narcolepsy>.State dead;
	}
}
