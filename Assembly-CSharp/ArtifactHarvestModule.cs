using System;
using UnityEngine;

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
		this.not_grounded.DefaultState(this.not_grounded.not_harvesting).EventHandler(GameHashes.ClusterLocationChanged, (ArtifactHarvestModule.StatesInstance smi) => Game.Instance, delegate(ArtifactHarvestModule.StatesInstance smi)
		{
			smi.CheckIfCanHarvest();
		}).EventHandler(GameHashes.OnStorageChange, delegate(ArtifactHarvestModule.StatesInstance smi)
		{
			smi.CheckIfCanHarvest();
		})
			.TagTransition(GameTags.RocketNotOnGround, this.grounded, true);
		this.not_grounded.not_harvesting.PlayAnim("loaded").ParamTransition<bool>(this.canHarvest, this.not_grounded.harvesting, GameStateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.IsTrue);
		this.not_grounded.harvesting.PlayAnim("deploying").Update(delegate(ArtifactHarvestModule.StatesInstance smi, float dt)
		{
			smi.HarvestFromPOI(dt);
		}, UpdateRate.SIM_4000ms, false).ParamTransition<bool>(this.canHarvest, this.not_grounded.not_harvesting, GameStateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.IsFalse);
	}

	public StateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.BoolParameter canHarvest;

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

		public void HarvestFromPOI(float dt)
		{
			ClusterGridEntity poiatCurrentLocation = base.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().GetPOIAtCurrentLocation();
			if (poiatCurrentLocation.IsNullOrDestroyed())
			{
				return;
			}
			ArtifactPOIStates.Instance smi = poiatCurrentLocation.GetSMI<ArtifactPOIStates.Instance>();
			if ((poiatCurrentLocation.GetComponent<ArtifactPOIClusterGridEntity>() || poiatCurrentLocation.GetComponent<HarvestablePOIClusterGridEntity>()) && !smi.IsNullOrDestroyed())
			{
				bool flag = false;
				string artifactToHarvest = smi.GetArtifactToHarvest();
				if (artifactToHarvest != null)
				{
					GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(artifactToHarvest), base.transform.position);
					gameObject.SetActive(true);
					this.receptacle.ForceDeposit(gameObject);
					this.storage.Store(gameObject, false, false, true, false);
					smi.HarvestArtifact();
					if (smi.configuration.DestroyOnHarvest())
					{
						flag = true;
					}
					if (flag)
					{
						poiatCurrentLocation.gameObject.DeleteObject();
					}
				}
			}
		}

		public bool CheckIfCanHarvest()
		{
			Clustercraft component = base.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			if (component == null)
			{
				return false;
			}
			ClusterGridEntity poiatCurrentLocation = component.GetPOIAtCurrentLocation();
			if (poiatCurrentLocation != null && (poiatCurrentLocation.GetComponent<ArtifactPOIClusterGridEntity>() || poiatCurrentLocation.GetComponent<HarvestablePOIClusterGridEntity>()))
			{
				ArtifactPOIStates.Instance smi = poiatCurrentLocation.GetSMI<ArtifactPOIStates.Instance>();
				if (smi != null && smi.CanHarvestArtifact() && this.receptacle.Occupant == null)
				{
					base.sm.canHarvest.Set(true, this);
					return true;
				}
			}
			base.sm.canHarvest.Set(false, this);
			return false;
		}

		[MyCmpReq]
		private Storage storage;

		[MyCmpReq]
		private SingleEntityReceptacle receptacle;
	}
}
