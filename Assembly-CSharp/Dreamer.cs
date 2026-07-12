using System;

public class Dreamer : GameStateMachine<Dreamer, Dreamer.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.notDreaming;
		this.notDreaming.OnSignal(this.startDreaming, this.dreaming, (Dreamer.Instance smi) => smi.currentDream != null);
		this.dreaming.Enter(new StateMachine<Dreamer, Dreamer.Instance, IStateMachineTarget, object>.State.Callback(Dreamer.PrepareDream)).OnSignal(this.stopDreaming, this.notDreaming).Update(new Action<Dreamer.Instance, float>(this.UpdateDream), UpdateRate.SIM_EVERY_TICK, false)
			.Exit(new StateMachine<Dreamer, Dreamer.Instance, IStateMachineTarget, object>.State.Callback(this.RemoveDream));
	}

	private void RemoveDream(Dreamer.Instance smi)
	{
		smi.SetDream(null);
		NameDisplayScreen.Instance.StopDreaming(smi.gameObject);
	}

	private void UpdateDream(Dreamer.Instance smi, float dt)
	{
		NameDisplayScreen.Instance.DreamTick(smi.gameObject, dt);
	}

	private static void PrepareDream(Dreamer.Instance smi)
	{
		NameDisplayScreen.Instance.SetDream(smi.gameObject, smi.currentDream);
	}

	public StateMachine<Dreamer, Dreamer.Instance, IStateMachineTarget, object>.Signal stopDreaming;

	public StateMachine<Dreamer, Dreamer.Instance, IStateMachineTarget, object>.Signal startDreaming;

	public GameStateMachine<Dreamer, Dreamer.Instance, IStateMachineTarget, object>.State notDreaming;

	public GameStateMachine<Dreamer, Dreamer.Instance, IStateMachineTarget, object>.State dreaming;

	public class DreamingState : GameStateMachine<Dreamer, Dreamer.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<Dreamer, Dreamer.Instance, IStateMachineTarget, object>.State hidden;

		public GameStateMachine<Dreamer, Dreamer.Instance, IStateMachineTarget, object>.State visible;
	}

	public new class Instance : GameStateMachine<Dreamer, Dreamer.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			NameDisplayScreen.Instance.RegisterComponent(base.gameObject, this, false);
		}

		public void SetDream(Dream dream)
		{
			this.currentDream = dream;
		}

		public void StartDreaming()
		{
			base.sm.startDreaming.Trigger(base.smi);
		}

		public void StopDreaming()
		{
			this.SetDream(null);
			base.sm.stopDreaming.Trigger(base.smi);
		}

		public Dream currentDream;
	}
}
