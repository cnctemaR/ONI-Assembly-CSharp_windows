using System;

public class ArtifactHarvestModule : GameStateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.grounded;
		this.root.Enter(delegate(ArtifactHarvestModule.StatesInstance smi)
		{
			smi.CheckIfCanHarvest();
		});
		this.grounded.TagTransition(GameTags.RocketNotOnGround, this.not_grounded, false);
		this.not_grounded.DefaultState(this.not_grounded.not_harvesting).EventHandler(GameHashes.ClusterLocationChanged, (ArtifactHarvestModule.StatesInstance smi) => Game.Instance, new GameStateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.GameEvent.Callback(ArtifactHarvestModule.OnAnythingChangingLocationsInSpace)).EventHandler(GameHashes.OnStorageChange, delegate(ArtifactHarvestModule.StatesInstance smi)
		{
			smi.CheckIfCanHarvest();
		})
			.TagTransition(GameTags.RocketNotOnGround, this.grounded, true);
		this.not_grounded.not_harvesting.PlayAnim("loaded").ParamTransition<bool>(this.canHarvest, this.not_grounded.harvesting, GameStateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.IsTrue);
		this.not_grounded.harvesting.PlayAnim("deploying").Update(delegate(ArtifactHarvestModule.StatesInstance smi, float dt)
		{
			smi.HarvestFromHexCell(dt);
		}, UpdateRate.SIM_4000ms, false).ParamTransition<bool>(this.canHarvest, this.not_grounded.not_harvesting, GameStateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.IsFalse);
	}

	private static void OnAnythingChangingLocationsInSpace(ArtifactHarvestModule.StatesInstance smi, object obj)
	{
		if (obj == null)
		{
			return;
		}
		ClusterLocationChangedEvent clusterLocationChangedEvent = (ClusterLocationChangedEvent)obj;
		Clustercraft component = smi.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
		if (clusterLocationChangedEvent.entity == component)
		{
			smi.CheckIfCanHarvest();
		}
	}

	public StateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.BoolParameter canHarvest;

	public StateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.TargetParameter entityTarget;

	public GameStateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.State grounded;

	public ArtifactHarvestModule.NotGroundedStates not_grounded;

	public class Def : StateMachine.BaseDef
	{
	}

	public class NotGroundedStates : GameStateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.State
	{
		public GameStateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.State not_harvesting;

		public GameStateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.State harvesting;
	}

	public class StatesInstance : GameStateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.GameInstance
	{
		public StatesInstance(IStateMachineTarget master, ArtifactHarvestModule.Def def)
			: base(master, def)
		{
		}

		public void HarvestFromHexCell(float dt)
		{
			Clustercraft component = base.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			StarmapHexCellInventory starmapHexCellInventory = ClusterGrid.Instance.AddOrGetHexCellInventory(component.Location);
			StarmapHexCellInventory.SerializedItem serializedItem = starmapHexCellInventory.Items.Find((StarmapHexCellInventory.SerializedItem item) => item.IsEntity && Assets.GetPrefab(item.ID).HasTag(GameTags.Artifact));
			if (serializedItem != null)
			{
				PrimaryElement primaryElement = starmapHexCellInventory.ExtractAndSpawnItem(serializedItem.ID);
				this.receptacle.ForceDeposit(primaryElement.gameObject);
				this.storage.Store(primaryElement.gameObject, false, false, true, false);
				return;
			}
		}

		public bool CheckIfCanHarvest()
		{
			Clustercraft component = base.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			if (component == null)
			{
				return false;
			}
			if (this.receptacle.Occupant != null)
			{
				base.sm.canHarvest.Set(false, this, false);
				return false;
			}
			ClusterGridEntity poiatCurrentLocation = component.GetPOIAtCurrentLocation();
			if (ClusterGrid.Instance.AddOrGetHexCellInventory(component.Location).Items.Find((StarmapHexCellInventory.SerializedItem item) => item.IsEntity && Assets.GetPrefab(item.ID).HasTag(GameTags.Artifact)) != null)
			{
				base.sm.canHarvest.Set(true, this, false);
				return true;
			}
			if (poiatCurrentLocation != null && (poiatCurrentLocation.GetComponent<ArtifactPOIClusterGridEntity>() || poiatCurrentLocation.GetComponent<HarvestablePOIClusterGridEntity>()))
			{
				ArtifactPOIStates.Instance smi = poiatCurrentLocation.GetSMI<ArtifactPOIStates.Instance>();
				if (smi != null && smi.HasArtifactAvailableInHexCell())
				{
					base.sm.canHarvest.Set(true, this, false);
					return true;
				}
			}
			base.sm.canHarvest.Set(false, this, false);
			return false;
		}

		[MyCmpReq]
		private Storage storage;

		[MyCmpReq]
		private SingleEntityReceptacle receptacle;
	}
}
