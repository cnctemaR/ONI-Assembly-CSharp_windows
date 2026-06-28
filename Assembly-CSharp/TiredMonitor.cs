using System;

public class TiredMonitor : GameStateMachine<TiredMonitor, TiredMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.EventTransition(GameHashes.SleepFail, this.tired, null);
		this.tired.Enter(delegate(TiredMonitor.Instance smi)
		{
			smi.SetInterruptedDay();
		}).EventTransition(GameHashes.NewDay, (TiredMonitor.Instance smi) => GameClock.Instance, this.root, (TiredMonitor.Instance smi) => smi.AllowInterruptClear()).ToggleExpression(Db.Get().Expressions.Tired, null)
			.ToggleAnims("anim_loco_walk_slouch", 0f)
			.ToggleAnims("anim_idle_slouch", 0f)
			.ToggleEffect("InterruptedSleep");
	}

	public GameStateMachine<TiredMonitor, TiredMonitor.Instance, IStateMachineTarget>.State tired;

	public new class Instance : GameStateMachine<TiredMonitor, TiredMonitor.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void SetInterruptedDay()
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

		public int interruptedDay = -1;
	}
}
