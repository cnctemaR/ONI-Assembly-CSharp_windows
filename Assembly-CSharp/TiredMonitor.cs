using System;

public class TiredMonitor : GameStateMachine<TiredMonitor, TiredMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.EventTransition(GameHashes.SleepFail, this.tired, null);
		this.tired.Enter(delegate(TiredMonitor.Instance smi)
		{
			smi.SetInterruptDay();
		}).EventTransition(GameHashes.NewDay, (TiredMonitor.Instance smi) => GameClock.Instance, this.root, (TiredMonitor.Instance smi) => smi.AllowInterruptClear()).ToggleExpression(Db.Get().Expressions.Tired, null)
			.ToggleAnims("anim_loco_walk_slouch_kanim", 0f)
			.ToggleAnims("anim_idle_slouch_kanim", 0f);
	}

	public GameStateMachine<TiredMonitor, TiredMonitor.Instance, IStateMachineTarget, object>.State tired;

	public new class Instance : GameStateMachine<TiredMonitor, TiredMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void SetInterruptDay()
		{
			this.interruptedDay = GameClock.Instance.GetDay();
		}

		public bool AllowInterruptClear()
		{
			bool flag = GameClock.Instance.GetDay() > this.interruptedDay + 1;
			if (flag)
			{
				this.interruptedDay = -1;
			}
			return flag;
		}

		public int disturbedDay = -1;

		public int interruptedDay = -1;
	}
}
