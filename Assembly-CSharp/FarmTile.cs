using System;

public class FarmTile : StateMachineComponent<FarmTile.SMInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.storage.choreType = Db.Get().ChoreTypes.FetchCritical;
		base.smi.StartSM();
	}

	[MyCmpReq]
	private PlantablePlot plantablePlot;

	[MyCmpReq]
	private Storage storage;

	public class SMInstance : GameStateMachine<FarmTile.States, FarmTile.SMInstance, FarmTile, object>.GameInstance
	{
		public SMInstance(FarmTile master)
			: base(master)
		{
		}

		public bool HasWater()
		{
			PrimaryElement primaryElement = base.master.storage.FindPrimaryElement(SimHashes.Water);
			return primaryElement != null && primaryElement.Mass > 0f;
		}
	}

	public class States : GameStateMachine<FarmTile.States, FarmTile.SMInstance, FarmTile>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.empty;
			this.empty.EventTransition(GameHashes.OccupantChanged, this.full, (FarmTile.SMInstance smi) => smi.master.plantablePlot.Occupant != null);
			this.empty.wet.EventTransition(GameHashes.OnStorageChange, this.empty.dry, (FarmTile.SMInstance smi) => !smi.HasWater());
			this.empty.dry.EventTransition(GameHashes.OnStorageChange, this.empty.wet, (FarmTile.SMInstance smi) => !smi.HasWater());
			this.full.EventTransition(GameHashes.OccupantChanged, this.empty, (FarmTile.SMInstance smi) => smi.master.plantablePlot.Occupant == null);
			this.full.wet.EventTransition(GameHashes.OnStorageChange, this.full.dry, (FarmTile.SMInstance smi) => !smi.HasWater());
			this.full.dry.EventTransition(GameHashes.OnStorageChange, this.full.wet, (FarmTile.SMInstance smi) => !smi.HasWater());
		}

		public FarmTile.States.FarmStates empty;

		public FarmTile.States.FarmStates full;

		public class FarmStates : GameStateMachine<FarmTile.States, FarmTile.SMInstance, FarmTile, object>.State
		{
			public GameStateMachine<FarmTile.States, FarmTile.SMInstance, FarmTile, object>.State wet;

			public GameStateMachine<FarmTile.States, FarmTile.SMInstance, FarmTile, object>.State dry;
		}
	}
}
