using System;
using UnityEngine;

public class FlowerVase : StateMachineComponent<FlowerVase.SMInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
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

	public class SMInstance : GameStateMachine<FlowerVase.States, FlowerVase.SMInstance, FlowerVase, object>.GameInstance
	{
		public SMInstance(FlowerVase master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<FlowerVase.States, FlowerVase.SMInstance, FlowerVase>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.empty;
			this.empty.EventTransition(GameHashes.OccupantChanged, this.full, (FlowerVase.SMInstance smi) => smi.master.plantablePlot.Occupant != null).PlayAnim("off");
			this.full.EventTransition(GameHashes.OccupantChanged, this.empty, (FlowerVase.SMInstance smi) => smi.master.plantablePlot.Occupant == null).PlayAnim("on");
		}

		public GameStateMachine<FlowerVase.States, FlowerVase.SMInstance, FlowerVase, object>.State empty;

		public GameStateMachine<FlowerVase.States, FlowerVase.SMInstance, FlowerVase, object>.State full;
	}
}
