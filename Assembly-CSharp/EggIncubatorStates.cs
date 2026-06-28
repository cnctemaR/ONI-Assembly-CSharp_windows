using System;

public class EggIncubatorStates : GameStateMachine<EggIncubatorStates, EggIncubatorStates.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.empty;
		this.empty.PlayAnim("off", KAnim.PlayMode.Loop).EventTransition(GameHashes.OccupantChanged, this.occupied, (EggIncubatorStates.Instance smi) => smi.GetComponent<EggIncubator>().Occupant != null);
		this.occupied.DefaultState(this.occupied.unpowered).EventTransition(GameHashes.OccupantChanged, this.empty, (EggIncubatorStates.Instance smi) => smi.GetComponent<EggIncubator>().Occupant == null).ParamTransition<bool>(this.readyToHatch, this.occupied.readytohatch, (EggIncubatorStates.Instance smi, bool p) => p);
		this.occupied.unpowered_pre.PlayAnim("no_power_pre").OnAnimQueueComplete(this.occupied.unpowered);
		this.occupied.unpowered.PlayAnim("no_power_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.OperationalChanged, this.occupied.incubating, (EggIncubatorStates.Instance smi) => smi.GetComponent<Operational>().IsOperational);
		this.occupied.incubating.PlayAnim("no_power_post").QueueAnim("working_loop", true, null).EventTransition(GameHashes.OperationalChanged, this.occupied.unpowered_pre, (EggIncubatorStates.Instance smi) => !smi.GetComponent<Operational>().IsOperational);
		this.occupied.readytohatch.PlayAnim("working_pst").QueueAnim("ready_to_hatch_loop", true, null).OnSignal(this.startHatch, this.occupied.hatch);
		this.occupied.hatch.PlayAnim("hatching").Exit("CompleteHatch", delegate(EggIncubatorStates.Instance smi)
		{
			smi.GetComponent<EggIncubator>().CompleteHatch();
		}).OnAnimQueueComplete(this.empty);
	}

	public StateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.BoolParameter readyToHatch;

	public StateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.Signal startHatch;

	public GameStateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.State empty;

	public EggIncubatorStates.OccupiedStates occupied;

	public class OccupiedStates : GameStateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.State incubating;

		public GameStateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.State unpowered_pre;

		public GameStateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.State unpowered;

		public GameStateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.State readytohatch;

		public GameStateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.State hatch;
	}

	public new class Instance : GameStateMachine<EggIncubatorStates, EggIncubatorStates.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}
	}
}
