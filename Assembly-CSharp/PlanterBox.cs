using System;
using UnityEngine;

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

	[MyCmpReq]
	private BoxCollider2D boxCollider;

	public class SMInstance : GameStateMachine<PlanterBox.States, PlanterBox.SMInstance, PlanterBox>.GameInstance
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
			this.empty.EventTransition(GameHashes.OccupantChanged, this.full, (PlanterBox.SMInstance smi) => smi.master.plantablePlot.Occupant != null).PlayAnim("off", KAnim.PlayMode.Once, null).Enter(delegate(PlanterBox.SMInstance smi)
			{
				smi.master.boxCollider.size = new Vector2(1f, 3f);
				smi.master.boxCollider.offset = new Vector2(0f, 1.5f);
			});
			this.full.EventTransition(GameHashes.OccupantChanged, this.empty, (PlanterBox.SMInstance smi) => smi.master.plantablePlot.Occupant == null).PlayAnim("on", KAnim.PlayMode.Once, null).Enter(delegate(PlanterBox.SMInstance smi)
			{
				smi.master.boxCollider.size = new Vector2(1f, 1f);
				smi.master.boxCollider.offset = new Vector2(0f, 0.5f);
			});
		}

		public GameStateMachine<PlanterBox.States, PlanterBox.SMInstance, PlanterBox>.State empty;

		public GameStateMachine<PlanterBox.States, PlanterBox.SMInstance, PlanterBox>.State full;
	}
}
