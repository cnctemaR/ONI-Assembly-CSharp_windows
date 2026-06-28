using System;
using UnityEngine;

public class Restable : Usable
{
	public override void StartUsing(User user)
	{
		Restable.StatesInstance statesInstance = new Restable.StatesInstance(this, user.gameObject);
		base.StartUsing(statesInstance, user);
	}

	public class StatesInstance : GameStateMachine<Restable.States, Restable.StatesInstance, Restable, object>.GameInstance
	{
		public StatesInstance(Restable master, GameObject sleeper)
			: base(master)
		{
			this.staminaMonitor = sleeper.GetSMI<StaminaMonitor.Instance>();
			base.sm.sleeper.Set(sleeper, base.smi);
		}

		public bool ShouldExitSleep()
		{
			return !this.IsNarcolepsing() && this.staminaMonitor.ShouldExitSleep();
		}

		public bool IsNarcolepsing()
		{
			Narcolepsy component = base.sm.sleeper.Get(base.smi).GetComponent<Narcolepsy>();
			return component != null && component.IsNarcolepsing();
		}

		public string GetAnims()
		{
			return (base.sm.sleeper.Get<Navigator>(base.smi).CurrentNavType != NavType.Ladder) ? "anim_sleep_floor_kanim" : "anim_sleep_ladder_kanim";
		}

		public StaminaMonitor.Instance staminaMonitor;
	}

	public class States : GameStateMachine<Restable.States, Restable.StatesInstance, Restable>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.working.pre;
			base.Target(this.sleeper);
			this.working.ToggleAnims((Restable.StatesInstance smi) => smi.GetAnims());
			this.working.pre.PlayAnim("working_pre", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.working.loop);
			this.working.loop.Enter(delegate(Restable.StatesInstance smi)
			{
				if (smi.IsNarcolepsing())
				{
					smi.GoTo(this.working.loop.narcoleptic);
				}
				else
				{
					smi.GoTo(this.working.loop.normal);
				}
			}).EventTransition(GameHashes.SleepFail, this.working.interrupt, null).Transition(this.working.pst, (Restable.StatesInstance smi) => smi.ShouldExitSleep());
			this.working.loop.normal.ToggleEffect("Sleep").PlayAnim("working_loop", KAnim.PlayMode.Loop, null);
			this.working.loop.narcoleptic.ToggleEffect("NarcolepticSleep").PlayAnim("working_loop", KAnim.PlayMode.Loop, null);
			this.working.interrupt.Enter(delegate(Restable.StatesInstance smi)
			{
				this.sleeper.Get<KBatchedAnimController>(smi).Play("interrupt", KAnim.PlayMode.Once, 1f, 0f);
			}).OnAnimQueueComplete(this.working.loop);
			this.working.pst.PlayAnim("working_pst", KAnim.PlayMode.Once, null).AddEffect("SoreBack").OnAnimQueueComplete(null);
		}

		public StateMachine<Restable.States, Restable.StatesInstance, Restable, object>.TargetParameter sleeper;

		public Restable.States.WorkingState working;

		public class SleepState : GameStateMachine<Restable.States, Restable.StatesInstance, Restable, object>.State
		{
			public GameStateMachine<Restable.States, Restable.StatesInstance, Restable, object>.State normal;

			public GameStateMachine<Restable.States, Restable.StatesInstance, Restable, object>.State narcoleptic;
		}

		public class WorkingState : GameStateMachine<Restable.States, Restable.StatesInstance, Restable, object>.State
		{
			public GameStateMachine<Restable.States, Restable.StatesInstance, Restable, object>.State pre;

			public Restable.States.SleepState loop;

			public GameStateMachine<Restable.States, Restable.StatesInstance, Restable, object>.State pst;

			public GameStateMachine<Restable.States, Restable.StatesInstance, Restable, object>.State interrupt;
		}
	}
}
