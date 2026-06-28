using System;
using UnityEngine;

public class SleepChore : Chore<SleepChore.StatesInstance>
{
	public SleepChore(IStateMachineTarget target, GameObject bed)
		: base(Db.Get().ChoreTypes.Sleep, target, target.GetComponent<ChoreProvider>(), false, null, null, null, int.MaxValue, false, true, 0)
	{
		this.smi = new SleepChore.StatesInstance(this, target.gameObject, bed);
		base.AddPrecondition(ChorePreconditions.IsNotRedAlert, null);
		base.AddPrecondition(ChorePreconditions.IsScheduledTime, Db.Get().ScheduleBlockTypes.Sleep);
		base.AddPrecondition(ChorePreconditions.IsOperational, bed);
	}

	public class StatesInstance : GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.GameInstance
	{
		public StatesInstance(SleepChore master, GameObject sleeper, GameObject bed)
			: base(master)
		{
			base.sm.sleeper.Set(sleeper, base.smi);
			base.sm.bed.Set(bed, base.smi);
		}
	}

	public class States : GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.approach;
			base.Target(this.sleeper);
			this.approach.InitializeStates(this.sleeper, this.bed, this.sleep, null, null, null);
			this.sleep.DefaultState(this.sleep.comfortable).DoSleep(this.sleeper, this.bed, this.success, null);
			this.sleep.comfortable.EventTransition(GameHashes.SleepFail, this.sleep.irritated, null);
			this.sleep.irritated.PlayAnim("interrupt", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.sleep.comfortable).Exit(delegate(SleepChore.StatesInstance smi)
			{
				smi.Play("working_loop", KAnim.PlayMode.Once);
			});
			this.success.ReturnSuccess();
		}

		public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.TargetParameter sleeper;

		public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.TargetParameter bed;

		public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.ApproachSubState<Approachable> approach;

		public SleepChore.States.SleepStates sleep;

		public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State success;

		public class SleepStates : GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State
		{
			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State comfortable;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State irritated;
		}
	}
}
