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

	public class SMInstance : GameStateMachine<FlowerVase.States, FlowerVase.SMInstance, FlowerVase>.GameInstance
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
			this.empty.EventTransition(GameHashes.OccupantChanged, this.full, (FlowerVase.SMInstance smi) => smi.master.plantablePlot.Occupant != null).PlayAnim("off", KAnim.PlayMode.Once, null).Enter(delegate(FlowerVase.SMInstance smi)
			{
				smi.master.boxCollider.size = new Vector2(1f, 2f);
				smi.master.boxCollider.offset = new Vector2(0f, 1f);
			});
			this.full.EventTransition(GameHashes.OccupantChanged, this.empty, (FlowerVase.SMInstance smi) => smi.master.plantablePlot.Occupant == null).PlayAnim("on", KAnim.PlayMode.Once, null).Enter(delegate(FlowerVase.SMInstance smi)
			{
				smi.master.boxCollider.size = new Vector2(1f, 1f);
				smi.master.boxCollider.offset = new Vector2(0f, 0.5f);
			});
		}

		public GameStateMachine<FlowerVase.States, FlowerVase.SMInstance, FlowerVase>.State empty;

		public GameStateMachine<FlowerVase.States, FlowerVase.SMInstance, FlowerVase>.State full;
	}
}
