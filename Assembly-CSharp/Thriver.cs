using System;

[SkipSaveFileSerialization]
public class Thriver : StateMachineComponent<Thriver.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	public class StatesInstance : GameStateMachine<Thriver.States, Thriver.StatesInstance, Thriver, object>.GameInstance
	{
		public StatesInstance(Thriver master)
			: base(master)
		{
		}

		public bool IsStressed()
		{
			StressMonitor.Instance smi = base.master.GetSMI<StressMonitor.Instance>();
			return smi != null && smi.IsStressed();
		}
	}

	public class States : GameStateMachine<Thriver.States, Thriver.StatesInstance, Thriver>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.root.EventTransition(GameHashes.NotStressed, this.idle, null).EventTransition(GameHashes.Stressed, this.stressed, null).EventTransition(GameHashes.StressedHadEnough, this.stressed, null)
				.Enter(delegate(Thriver.StatesInstance smi)
				{
					StressMonitor.Instance smi2 = smi.master.GetSMI<StressMonitor.Instance>();
					if (smi2 != null && smi2.IsStressed())
					{
						smi.GoTo(this.stressed);
					}
				});
			this.idle.DoNothing();
			this.stressed.ToggleEffect("Thriver");
			this.toostressed.DoNothing();
		}

		public GameStateMachine<Thriver.States, Thriver.StatesInstance, Thriver, object>.State idle;

		public GameStateMachine<Thriver.States, Thriver.StatesInstance, Thriver, object>.State stressed;

		public GameStateMachine<Thriver.States, Thriver.StatesInstance, Thriver, object>.State toostressed;
	}
}
