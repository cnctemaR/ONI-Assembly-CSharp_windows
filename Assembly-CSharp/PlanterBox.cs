using System;

[SkipSaveFileSerialization]
public class PlanterBox : StateMachineComponent<PlanterBox.SMInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.GetComponent<Storage>().choreType = Db.Get().ChoreTypes.FetchCritical;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	[MyCmpReq]
	private PlantablePlot plantablePlot;

	public class SMInstance : GameStateMachine<PlanterBox.States, PlanterBox.SMInstance, PlanterBox, object>.GameInstance
	{
		public SMInstance(PlanterBox master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<PlanterBox.States, PlanterBox.SMInstance, PlanterBox>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.empty;
			this.empty.EventTransition(GameHashes.OccupantChanged, this.full, (PlanterBox.SMInstance smi) => smi.master.plantablePlot.Occupant != null).PlayAnim("off");
			this.full.EventTransition(GameHashes.OccupantChanged, this.empty, (PlanterBox.SMInstance smi) => smi.master.plantablePlot.Occupant == null).PlayAnim("on");
		}

		public GameStateMachine<PlanterBox.States, PlanterBox.SMInstance, PlanterBox, object>.State empty;

		public GameStateMachine<PlanterBox.States, PlanterBox.SMInstance, PlanterBox, object>.State full;
	}
}
