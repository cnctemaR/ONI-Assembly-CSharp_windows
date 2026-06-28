using System;
using System.Collections.Generic;
using UnityEngine;

public class MedicinalHerb : StateMachineComponent<MedicinalHerb.StatesInstance>, IGameObjectEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.crop = base.GetComponent<Crop>();
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

	public List<string> GetEffectDescriptions(GameObject go)
	{
		List<string> list = new List<string>();
		Crop component = go.GetComponent<Crop>();
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

	public class StatesInstance : GameStateMachine<MedicinalHerb.States, MedicinalHerb.StatesInstance, MedicinalHerb>.GameInstance
	{
		public StatesInstance(MedicinalHerb smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<MedicinalHerb.States, MedicinalHerb.StatesInstance, MedicinalHerb>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.alive;
			base.serializable = true;
			this.dead.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Dead).Enter(delegate(MedicinalHerb.StatesInstance smi)
			{
				GameUtil.KInstantiate(EffectPrefabs.Instance.PlantDeath, smi.master.transform.position, Grid.SceneLayer.FXFront, SceneOrganizer.Instance.GetFolder(Folder.FX), null, 0);
				smi.master.Trigger(1623392196, null);
				smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
				global::UnityEngine.Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
				smi.Schedule(0.5f, new Action<object>(smi.master.DestroySelf), null);
			});
			this.blocked_from_growing.ToggleStatusItem(Db.Get().MiscStatusItems.RegionIsBlocked, null).EventTransition(GameHashes.EntombedChanged, this.alive.seed_grow, (MedicinalHerb.StatesInstance smi) => !smi.master.GetComponent<EntombVulnerable>().GetEntombed).EventTransition(GameHashes.TooColdWarning, this.alive.seed_grow, null)
				.EventTransition(GameHashes.TooHotWarning, this.alive.seed_grow, null);
			this.alive.InitializeStates(this.masterTarget, this.dead).DefaultState(this.alive.seed_grow).Enter(delegate(MedicinalHerb.StatesInstance smi)
			{
				if (smi.master.growing.Replanted && !this.alive.ForceUpdateStatus(smi.master.gameObject))
				{
					smi.GoTo(this.blocked_from_growing);
				}
			});
			this.alive.seed_grow.QueueAnim("seed_grow", false, null).EventTransition(GameHashes.AnimQueueComplete, this.alive.idle, null).EventTransition(GameHashes.Wilt, this.alive.wilting.wilting_pre, (MedicinalHerb.StatesInstance smi) => smi.master.wiltCondition.IsWilting())
				.EventTransition(GameHashes.CropReady, this.alive.fruiting.fruiting_pre, null);
			this.alive.idle.PlayAnim("idle_loop", KAnim.PlayMode.Loop, null).EventTransition(GameHashes.CropDepleted, this.dead, (MedicinalHerb.StatesInstance smi) => !smi.master.crop.CanGrow()).EventTransition(GameHashes.Wilt, this.alive.wilting.wilting_pre, (MedicinalHerb.StatesInstance smi) => smi.master.wiltCondition.IsWilting())
				.EventTransition(GameHashes.CropReady, this.alive.fruiting.fruiting_pre, null);
			this.alive.wilting.wilting_pre.PlayAnim("wilt_pre", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.alive.wilting.wilting).EventTransition(GameHashes.WiltRecover, this.alive.wilting.wilting_pst, null);
			this.alive.wilting.wilting.PlayAnim("idle_wilt_loop", KAnim.PlayMode.Loop, null).EventTransition(GameHashes.WiltRecover, this.alive.wilting.wilting_pst, null);
			this.alive.wilting.wilting_pst.PlayAnim("wilt_pst", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.alive.idle);
			this.alive.fruiting.EventTransition(GameHashes.Harvest, this.alive.fruiting.fruiting_harvest, null).EventHandler(GameHashes.Wilt, delegate(MedicinalHerb.StatesInstance smi)
			{
				smi.master.crop.SpawnFruit(null);
				smi.master.growing.ResetGrowth();
				smi.master.harvestable.SetCanBeHarvested(false);
				smi.GoTo(this.alive.wilting.wilting_pre);
			});
			this.alive.fruiting.fruiting_pre.PlayAnim("grow", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.alive.fruiting.fruiting_idle);
			this.alive.fruiting.fruiting_idle.PlayAnim("idle_bloom_loop", KAnim.PlayMode.Loop, null).Enter(delegate(MedicinalHerb.StatesInstance smi)
			{
				smi.master.harvestable.SetCanBeHarvested(true);
			});
			this.alive.fruiting.fruiting_harvest.PlayAnim("harvest", KAnim.PlayMode.Once, null).Enter(delegate(MedicinalHerb.StatesInstance smi)
			{
				smi.Schedule(0.4f, new Action<object>(smi.master.crop.SpawnFruit), null);
				smi.master.growing.ResetGrowth();
				smi.master.harvestable.SetCanBeHarvested(false);
			}).OnAnimQueueComplete(this.alive.idle);
		}

		public GameStateMachine<MedicinalHerb.States, MedicinalHerb.StatesInstance, MedicinalHerb>.State blocked_from_growing;

		public MedicinalHerb.States.AliveStates alive;

		public GameStateMachine<MedicinalHerb.States, MedicinalHerb.StatesInstance, MedicinalHerb>.State dead;

		public class AliveStates : GameStateMachine<MedicinalHerb.States, MedicinalHerb.StatesInstance, MedicinalHerb>.PlantAliveSubState
		{
			public GameStateMachine<MedicinalHerb.States, MedicinalHerb.StatesInstance, MedicinalHerb>.State seed_grow;

			public GameStateMachine<MedicinalHerb.States, MedicinalHerb.StatesInstance, MedicinalHerb>.State idle;

			public MedicinalHerb.States.FruitingState fruiting;

			public MedicinalHerb.States.WiltingState wilting;

			public GameStateMachine<MedicinalHerb.States, MedicinalHerb.StatesInstance, MedicinalHerb>.State destroy;
		}

		public class FruitingState : GameStateMachine<MedicinalHerb.States, MedicinalHerb.StatesInstance, MedicinalHerb>.State
		{
			public GameStateMachine<MedicinalHerb.States, MedicinalHerb.StatesInstance, MedicinalHerb>.State fruiting_pre;

			public GameStateMachine<MedicinalHerb.States, MedicinalHerb.StatesInstance, MedicinalHerb>.State fruiting_idle;

			public GameStateMachine<MedicinalHerb.States, MedicinalHerb.StatesInstance, MedicinalHerb>.State fruiting_harvest;
		}

		public class WiltingState : GameStateMachine<MedicinalHerb.States, MedicinalHerb.StatesInstance, MedicinalHerb>.State
		{
			public GameStateMachine<MedicinalHerb.States, MedicinalHerb.StatesInstance, MedicinalHerb>.State wilting_pre;

			public GameStateMachine<MedicinalHerb.States, MedicinalHerb.StatesInstance, MedicinalHerb>.State wilting;

			public GameStateMachine<MedicinalHerb.States, MedicinalHerb.StatesInstance, MedicinalHerb>.State wilting_pst;
		}
	}
}
