using System;
using System.Collections.Generic;
using UnityEngine;

public class BasicSingleHarvestPlant : StateMachineComponent<BasicSingleHarvestPlant.StatesInstance>, IGameObjectEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.growing.enabled = false;
		base.smi.animController.randomiseLoopedOffset = true;
		base.smi.StartSM();
	}

	protected void DestroySelf(object callbackParam)
	{
		CreatureHelpers.DeselectCreature(base.gameObject);
		Util.KDestroyGameObject(base.gameObject);
	}

	public int DescriptionOrder { get; set; }

	public List<Descriptor> GetRequirementDescriptions(GameObject go)
	{
		return null;
	}

	public List<string> GetEffectDescriptions(GameObject plant_prefab)
	{
		List<string> list = new List<string>();
		Crop component = plant_prefab.GetComponent<Crop>();
		List<string> cropHarvestDetails = GameUtil.GetCropHarvestDetails(component);
		if (cropHarvestDetails.Count > 0)
		{
			for (int i = 0; i < cropHarvestDetails.Count; i++)
			{
				list.Add(cropHarvestDetails[i]);
			}
		}
		return list;
	}

	[MyCmpReq]
	private Crop crop;

	[MyCmpReq]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private Growing growing;

	[MyCmpReq]
	private Harvestable harvestable;

	[MyCmpReq]
	private KAnimControllerBase animController;

	public class StatesInstance : GameStateMachine<BasicSingleHarvestPlant.States, BasicSingleHarvestPlant.StatesInstance, BasicSingleHarvestPlant>.GameInstance
	{
		public StatesInstance(BasicSingleHarvestPlant smi)
			: base(smi)
		{
		}

		public bool IsOld()
		{
			return base.master.growing.PercentOldAge() > 0.5f;
		}

		public int WiltStage()
		{
			float num = base.master.growing.PercentGrown();
			if (num < 0.75f)
			{
				return 1;
			}
			if (num < 1f)
			{
				return 2;
			}
			return 3;
		}
	}

	public class States : GameStateMachine<BasicSingleHarvestPlant.States, BasicSingleHarvestPlant.StatesInstance, BasicSingleHarvestPlant>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = true;
			default_state = this.alive;
			this.dead.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Dead).Enter(delegate(BasicSingleHarvestPlant.StatesInstance smi)
			{
				GameUtil.KInstantiate(EffectPrefabs.Instance.PlantDeath, smi.master.transform.position, Grid.SceneLayer.FXFront, SceneOrganizer.Instance.GetFolder(Folder.FX), null, 0);
				smi.master.Trigger(1623392196, null);
				smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
				global::UnityEngine.Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
				smi.Schedule(0.5f, new Action<object>(smi.master.DestroySelf), null);
			});
			this.blocked_from_growing.ToggleStatusItem(Db.Get().MiscStatusItems.RegionIsBlocked, null).EventTransition(GameHashes.EntombedChanged, this.alive.idle, (BasicSingleHarvestPlant.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject)).EventTransition(GameHashes.TooColdWarning, this.alive.idle, (BasicSingleHarvestPlant.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject))
				.EventTransition(GameHashes.TooHotWarning, this.alive.idle, (BasicSingleHarvestPlant.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject));
			this.alive.InitializeStates(this.masterTarget, this.dead).DefaultState(this.alive.idle).Enter(delegate(BasicSingleHarvestPlant.StatesInstance smi)
			{
				if (smi.master.growing.Replanted && !this.alive.ForceUpdateStatus(smi.master.gameObject))
				{
					smi.GoTo(this.blocked_from_growing);
				}
			})
				.ToggleComponent<Growing>();
			this.alive.idle.EventTransition(GameHashes.Wilt, this.alive.wilting, (BasicSingleHarvestPlant.StatesInstance smi) => smi.master.wiltCondition.IsWilting()).EventTransition(GameHashes.CropDepleted, this.dead, (BasicSingleHarvestPlant.StatesInstance smi) => !smi.master.crop.CanGrow()).EventTransition(GameHashes.Grow, this.alive.pre_fruiting, (BasicSingleHarvestPlant.StatesInstance smi) => smi.master.growing.IsGrown())
				.PlayAnim("grow", KAnim.PlayMode.Paused, null)
				.Enter(delegate(BasicSingleHarvestPlant.StatesInstance smi)
				{
					smi.master.animController.SetPositionPercent(smi.master.growing.PercentGrown());
				})
				.Update(delegate(BasicSingleHarvestPlant.StatesInstance smi)
				{
					smi.master.animController.SetPositionPercent(smi.master.growing.PercentGrown());
				});
			this.alive.pre_fruiting.QueueAnim("grow_pst", false, null).EventHandler(GameHashes.AnimQueueComplete, delegate(BasicSingleHarvestPlant.StatesInstance smi)
			{
				smi.GoTo(this.alive.fruiting);
			});
			this.alive.wilting.PlayAnim("wilt", KAnim.PlayMode.Loop, (BasicSingleHarvestPlant.StatesInstance smi) => smi.WiltStage().ToString()).EventTransition(GameHashes.WiltRecover, this.alive.idle, (BasicSingleHarvestPlant.StatesInstance smi) => !smi.master.wiltCondition.IsWilting());
			this.alive.fruiting.DefaultState(this.alive.fruiting.fruiting_idle).EventHandler(GameHashes.Wilt, delegate(BasicSingleHarvestPlant.StatesInstance smi)
			{
				smi.master.crop.SpawnFruit(null);
				smi.master.growing.ResetGrowth();
				smi.master.harvestable.SetCanBeHarvested(false);
				smi.GoTo(this.alive.wilting);
			}).EventTransition(GameHashes.Harvest, this.alive.fruiting.fruiting_harvest, null);
			this.alive.fruiting.fruiting_idle.PlayAnim("idle_full", KAnim.PlayMode.Loop, null).Enter(delegate(BasicSingleHarvestPlant.StatesInstance smi)
			{
				smi.master.harvestable.SetCanBeHarvested(true);
			}).Transition(this.alive.fruiting.fruiting_old, (BasicSingleHarvestPlant.StatesInstance smi) => smi.IsOld());
			this.alive.fruiting.fruiting_old.PlayAnim("wilt", KAnim.PlayMode.Loop, (BasicSingleHarvestPlant.StatesInstance smi) => smi.WiltStage().ToString()).Enter(delegate(BasicSingleHarvestPlant.StatesInstance smi)
			{
				smi.master.harvestable.SetCanBeHarvested(true);
			}).Transition(this.alive.fruiting.fruiting_idle, (BasicSingleHarvestPlant.StatesInstance smi) => !smi.IsOld());
			this.alive.fruiting.fruiting_harvest.PlayAnim("harvest", KAnim.PlayMode.Once, null).Enter(delegate(BasicSingleHarvestPlant.StatesInstance smi)
			{
				smi.Schedule(0.2f, new Action<object>(smi.master.crop.SpawnFruit), null);
				smi.master.growing.ResetGrowth();
				smi.master.harvestable.SetCanBeHarvested(false);
			}).OnAnimQueueComplete(this.alive.idle);
		}

		public GameStateMachine<BasicSingleHarvestPlant.States, BasicSingleHarvestPlant.StatesInstance, BasicSingleHarvestPlant>.State blocked_from_growing;

		public BasicSingleHarvestPlant.States.AliveStates alive;

		public GameStateMachine<BasicSingleHarvestPlant.States, BasicSingleHarvestPlant.StatesInstance, BasicSingleHarvestPlant>.State dead;

		public class AliveStates : GameStateMachine<BasicSingleHarvestPlant.States, BasicSingleHarvestPlant.StatesInstance, BasicSingleHarvestPlant>.PlantAliveSubState
		{
			public GameStateMachine<BasicSingleHarvestPlant.States, BasicSingleHarvestPlant.StatesInstance, BasicSingleHarvestPlant>.State idle;

			public GameStateMachine<BasicSingleHarvestPlant.States, BasicSingleHarvestPlant.StatesInstance, BasicSingleHarvestPlant>.State pre_fruiting;

			public GameStateMachine<BasicSingleHarvestPlant.States, BasicSingleHarvestPlant.StatesInstance, BasicSingleHarvestPlant>.State barren;

			public BasicSingleHarvestPlant.States.FruitingState fruiting;

			public GameStateMachine<BasicSingleHarvestPlant.States, BasicSingleHarvestPlant.StatesInstance, BasicSingleHarvestPlant>.State wilting;

			public GameStateMachine<BasicSingleHarvestPlant.States, BasicSingleHarvestPlant.StatesInstance, BasicSingleHarvestPlant>.State destroy;
		}

		public class FruitingState : GameStateMachine<BasicSingleHarvestPlant.States, BasicSingleHarvestPlant.StatesInstance, BasicSingleHarvestPlant>.State
		{
			public GameStateMachine<BasicSingleHarvestPlant.States, BasicSingleHarvestPlant.StatesInstance, BasicSingleHarvestPlant>.State fruiting_idle;

			public GameStateMachine<BasicSingleHarvestPlant.States, BasicSingleHarvestPlant.StatesInstance, BasicSingleHarvestPlant>.State fruiting_old;

			public GameStateMachine<BasicSingleHarvestPlant.States, BasicSingleHarvestPlant.StatesInstance, BasicSingleHarvestPlant>.State fruiting_harvest;
		}
	}
}
