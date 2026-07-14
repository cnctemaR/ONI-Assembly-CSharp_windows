using System;

public class UnderwaterBreathingStation : GameStateMachine<UnderwaterBreathingStation, UnderwaterBreathingStation.Instance, IStateMachineTarget, UnderwaterBreathingStation.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		this.root.EventHandler(GameHashes.OnStorageChange, new StateMachine<UnderwaterBreathingStation, UnderwaterBreathingStation.Instance, IStateMachineTarget, UnderwaterBreathingStation.Def>.State.Callback(UnderwaterBreathingStation.RefreshMeter));
		this.off.EventTransition(GameHashes.OperationalChanged, this.on, (UnderwaterBreathingStation.Instance smi) => smi.GetComponent<Operational>().IsOperational).Enter(new StateMachine<UnderwaterBreathingStation, UnderwaterBreathingStation.Instance, IStateMachineTarget, UnderwaterBreathingStation.Def>.State.Callback(UnderwaterBreathingStation.RemoveCells));
		this.on.EventTransition(GameHashes.OperationalChanged, this.off, (UnderwaterBreathingStation.Instance smi) => !smi.GetComponent<Operational>().IsOperational).Enter(new StateMachine<UnderwaterBreathingStation, UnderwaterBreathingStation.Instance, IStateMachineTarget, UnderwaterBreathingStation.Def>.State.Callback(UnderwaterBreathingStation.AddCells)).Exit(new StateMachine<UnderwaterBreathingStation, UnderwaterBreathingStation.Instance, IStateMachineTarget, UnderwaterBreathingStation.Def>.State.Callback(UnderwaterBreathingStation.RemoveCells));
	}

	private static void RefreshMeter(UnderwaterBreathingStation.Instance smi)
	{
		smi.RefreshMeter();
	}

	private static void AddCells(UnderwaterBreathingStation.Instance smi)
	{
		smi.location.MarkCells();
	}

	private static void RemoveCells(UnderwaterBreathingStation.Instance smi)
	{
		smi.location.UnmarkCells();
	}

	private const string METER_TARGET_NAME = "meter_target";

	private const string METER_ANIM_NAME = "meter";

	public GameStateMachine<UnderwaterBreathingStation, UnderwaterBreathingStation.Instance, IStateMachineTarget, UnderwaterBreathingStation.Def>.State off;

	public GameStateMachine<UnderwaterBreathingStation, UnderwaterBreathingStation.Instance, IStateMachineTarget, UnderwaterBreathingStation.Def>.State on;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<UnderwaterBreathingStation, UnderwaterBreathingStation.Instance, IStateMachineTarget, UnderwaterBreathingStation.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, UnderwaterBreathingStation.Def def)
			: base(master, def)
		{
			this.storage = base.GetComponent<Storage>();
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			this.meter = new MeterController(component, "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.BuildingBack, Array.Empty<string>());
		}

		public override void StartSM()
		{
			this.location = base.GetComponent<UnderwaterBreathingLocation>();
			base.StartSM();
			this.RefreshMeter();
		}

		protected override void OnCleanUp()
		{
			this.location.UnmarkCells();
			base.OnCleanUp();
		}

		public void RefreshMeter()
		{
			float num = this.storage.MassStored() / this.storage.capacityKg;
			this.meter.SetPositionPercent(num);
		}

		public UnderwaterBreathingLocation location;

		private Storage storage;

		private MeterController meter;
	}
}
