using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHarvestModule : GameStateMachine<ResourceHarvestModule, ResourceHarvestModule.StatesInstance, IStateMachineTarget, ResourceHarvestModule.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.grounded;
		this.root.Enter(delegate(ResourceHarvestModule.StatesInstance smi)
		{
			smi.CheckIfCanDrill();
		});
		this.grounded.TagTransition(GameTags.RocketNotOnGround, this.not_grounded, false).Enter(delegate(ResourceHarvestModule.StatesInstance smi)
		{
			smi.UpdateMeter(null);
		});
		this.not_grounded.DefaultState(this.not_grounded.not_drilling).EventHandler(GameHashes.ClusterLocationChanged, (ResourceHarvestModule.StatesInstance smi) => Game.Instance, delegate(ResourceHarvestModule.StatesInstance smi)
		{
			smi.CheckIfCanDrill();
		}).EventHandler(GameHashes.OnStorageChange, delegate(ResourceHarvestModule.StatesInstance smi)
		{
			smi.CheckIfCanDrill();
		})
			.TagTransition(GameTags.RocketNotOnGround, this.grounded, true);
		this.not_grounded.not_drilling.PlayAnim("loaded").ParamTransition<bool>(this.canHarvest, this.not_grounded.drilling, GameStateMachine<ResourceHarvestModule, ResourceHarvestModule.StatesInstance, IStateMachineTarget, ResourceHarvestModule.Def>.IsTrue).Enter(delegate(ResourceHarvestModule.StatesInstance smi)
		{
			ResourceHarvestModule.StatesInstance.RemoveHarvestStatusItems(smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.gameObject);
		})
			.Update(delegate(ResourceHarvestModule.StatesInstance smi, float dt)
			{
				smi.CheckIfCanDrill();
			}, UpdateRate.SIM_4000ms, false);
		this.not_grounded.drilling.PlayAnim("deploying").Exit(delegate(ResourceHarvestModule.StatesInstance smi)
		{
			smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().Trigger(939543986, null);
			ResourceHarvestModule.StatesInstance.RemoveHarvestStatusItems(smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.gameObject);
		}).Enter(delegate(ResourceHarvestModule.StatesInstance smi)
		{
			Clustercraft component = smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			component.AddTag(GameTags.RocketDrilling);
			component.Trigger(-1762453998, null);
			ResourceHarvestModule.StatesInstance.AddHarvestStatusItems(smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.gameObject, smi);
		})
			.Exit(delegate(ResourceHarvestModule.StatesInstance smi)
			{
				smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().RemoveTag(GameTags.RocketDrilling);
			})
			.Update(delegate(ResourceHarvestModule.StatesInstance smi, float dt)
			{
				smi.HarvestFromPOI(dt);
				this.lastHarvestTime.Set(Time.time, smi, false);
			}, UpdateRate.SIM_4000ms, false)
			.ParamTransition<bool>(this.canHarvest, this.not_grounded.not_drilling, GameStateMachine<ResourceHarvestModule, ResourceHarvestModule.StatesInstance, IStateMachineTarget, ResourceHarvestModule.Def>.IsFalse);
	}

	public StateMachine<ResourceHarvestModule, ResourceHarvestModule.StatesInstance, IStateMachineTarget, ResourceHarvestModule.Def>.BoolParameter canHarvest;

	public StateMachine<ResourceHarvestModule, ResourceHarvestModule.StatesInstance, IStateMachineTarget, ResourceHarvestModule.Def>.FloatParameter lastHarvestTime;

	public GameStateMachine<ResourceHarvestModule, ResourceHarvestModule.StatesInstance, IStateMachineTarget, ResourceHarvestModule.Def>.State grounded;

	public ResourceHarvestModule.NotGroundedStates not_grounded;

	public class Def : StateMachine.BaseDef
	{
		public float harvestSpeed;
	}

	public class NotGroundedStates : GameStateMachine<ResourceHarvestModule, ResourceHarvestModule.StatesInstance, IStateMachineTarget, ResourceHarvestModule.Def>.State
	{
		public GameStateMachine<ResourceHarvestModule, ResourceHarvestModule.StatesInstance, IStateMachineTarget, ResourceHarvestModule.Def>.State not_drilling;

		public GameStateMachine<ResourceHarvestModule, ResourceHarvestModule.StatesInstance, IStateMachineTarget, ResourceHarvestModule.Def>.State drilling;
	}

	public class StatesInstance : GameStateMachine<ResourceHarvestModule, ResourceHarvestModule.StatesInstance, IStateMachineTarget, ResourceHarvestModule.Def>.GameInstance
	{
		public StatesInstance(IStateMachineTarget master, ResourceHarvestModule.Def def)
			: base(master, def)
		{
			this.storage = base.GetComponent<Storage>();
			base.GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, new ConditionHasResource(this.storage, SimHashes.Diamond, 1000f));
			this.onStorageChangeHandle = base.Subscribe(-1697596308, new Action<object>(this.UpdateMeter));
			this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[] { "meter_target", "meter_fill", "meter_frame", "meter_OL" });
			KBatchedAnimTracker component = this.meter.gameObject.GetComponent<KBatchedAnimTracker>();
			component.matchParentOffset = true;
			component.forceAlwaysAlive = true;
			this.UpdateMeter(null);
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
			base.Unsubscribe(ref this.onStorageChangeHandle);
		}

		public void UpdateMeter(object data = null)
		{
			this.meter.SetPositionPercent(this.storage.MassStored() / this.storage.Capacity());
		}

		public void HarvestFromPOI(float dt)
		{
			Clustercraft component = base.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			if (!this.CheckIfCanDrill())
			{
				return;
			}
			ClusterGridEntity poiatCurrentLocation = component.GetPOIAtCurrentLocation();
			if (poiatCurrentLocation == null || poiatCurrentLocation.GetComponent<HarvestablePOIClusterGridEntity>() == null)
			{
				return;
			}
			StarmapHexCellInventory starmapHexCellInventory = ClusterGrid.Instance.AddOrGetHexCellInventory(component.Location);
			HarvestablePOIStates.Instance smi = poiatCurrentLocation.GetSMI<HarvestablePOIStates.Instance>();
			Dictionary<SimHashes, float> elementsWithWeights = smi.configuration.GetElementsWithWeights();
			float num = 0f;
			foreach (KeyValuePair<SimHashes, float> keyValuePair in elementsWithWeights)
			{
				num += keyValuePair.Value;
			}
			foreach (KeyValuePair<SimHashes, float> keyValuePair2 in elementsWithWeights)
			{
				Element element = ElementLoader.FindElementByHash(keyValuePair2.Key);
				if (!DiscoveredResources.Instance.IsDiscovered(element.tag))
				{
					DiscoveredResources.Instance.Discover(element.tag, element.GetMaterialCategoryTag());
				}
			}
			float num2 = Mathf.Min(this.GetMaxExtractKGFromDiamondAvailable(), base.def.harvestSpeed * dt);
			float num3 = 0f;
			foreach (KeyValuePair<SimHashes, float> keyValuePair3 in elementsWithWeights)
			{
				if (num3 >= num2)
				{
					break;
				}
				SimHashes key = keyValuePair3.Key;
				float num4 = keyValuePair3.Value / num;
				float num5 = base.def.harvestSpeed * dt * num4;
				Element element2 = ElementLoader.FindElementByHash(key);
				starmapHexCellInventory.AddItem(element2, num5);
				num3 += num5;
			}
			smi.DeltaPOICapacity(-num3);
			this.ConsumeDiamond(num3 * 0.05f);
			SaveGame.Instance.ColonyAchievementTracker.totalMaterialsHarvestFromPOI += num3;
		}

		public void ConsumeDiamond(float amount)
		{
			base.GetComponent<Storage>().ConsumeIgnoringDisease(SimHashes.Diamond.CreateTag(), amount);
		}

		public bool HasAnyAmountOfDiamond()
		{
			return base.GetComponent<Storage>().GetAmountAvailable(SimHashes.Diamond.CreateTag()) > 0f;
		}

		public float GetMaxExtractKGFromDiamondAvailable()
		{
			return base.GetComponent<Storage>().GetAmountAvailable(SimHashes.Diamond.CreateTag()) / 0.05f;
		}

		public bool CheckIfCanDrill()
		{
			Clustercraft component = base.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			if (component == null)
			{
				base.sm.canHarvest.Set(false, this, false);
				return false;
			}
			if (!this.HasAnyAmountOfDiamond())
			{
				base.sm.canHarvest.Set(false, this, false);
				return false;
			}
			ClusterGridEntity poiatCurrentLocation = component.GetPOIAtCurrentLocation();
			bool flag = false;
			if (poiatCurrentLocation != null && poiatCurrentLocation.GetComponent<HarvestablePOIClusterGridEntity>())
			{
				flag = poiatCurrentLocation.GetSMI<HarvestablePOIStates.Instance>().POICanBeHarvested();
			}
			base.sm.canHarvest.Set(flag, this, false);
			return flag;
		}

		public static void AddHarvestStatusItems(GameObject statusTarget, ResourceHarvestModule.StatesInstance smi)
		{
			statusTarget.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SpacePOIHarvesting, smi);
		}

		public static void RemoveHarvestStatusItems(GameObject statusTarget)
		{
			statusTarget.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SpacePOIHarvesting, false);
		}

		private MeterController meter;

		private Storage storage;

		private int onStorageChangeHandle = -1;
	}
}
