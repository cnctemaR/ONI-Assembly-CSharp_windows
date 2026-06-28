using System;
using UnityEngine;

public class StandardCropPlant : StateMachineComponent<StandardCropPlant.StatesInstance>
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

	public class StatesInstance : GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.GameInstance
	{
		public StatesInstance(StandardCropPlant master)
			: base(master)
		{
		}

		public bool IsOld()
		{
			return base.master.growing.PercentOldAge() > 0.5f;
		}

		public bool HasBeenHarvested()
		{
			return base.master.crop != null && base.master.crop.GetTimesHarvested() > 0;
		}

		public int WiltStage()
		{
			float num = base.master.growing.PercentOfCurrentHarvest();
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

	public class States : GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = true;
			default_state = this.alive;
			this.dead.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Dead).Enter(delegate(StandardCropPlant.StatesInstance smi)
			{
				GameUtil.KInstantiate(EffectPrefabs.Instance.PlantDeath, smi.master.transform.position, Grid.SceneLayer.FXFront, SceneOrganizer.Instance.GetFolder(Folder.FX), null, 0);
				smi.master.Trigger(1623392196, null);
				smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
				global::UnityEngine.Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
				smi.Schedule(0.5f, new Action<object>(smi.master.DestroySelf), null);
			});
			this.blocked_from_growing.ToggleStatusItem(Db.Get().MiscStatusItems.RegionIsBlocked, null).EventTransition(GameHashes.EntombedChanged, this.alive.idle, (StandardCropPlant.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject)).EventTransition(GameHashes.TooColdWarning, this.alive.idle, (StandardCropPlant.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject))
				.EventTransition(GameHashes.TooHotWarning, this.alive.idle, (StandardCropPlant.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject))
				.EventTransition(GameHashes.Uprooted, this.dead, (StandardCropPlant.StatesInstance smi) => UprootedMonitor.IsObjectUprooted(smi.master.gameObject));
			this.alive.InitializeStates(this.masterTarget, this.dead).DefaultState(this.alive.idle).Enter(delegate(StandardCropPlant.StatesInstance smi)
			{
				if (smi.master.growing.Replanted && !this.alive.ForceUpdateStatus(smi.master.gameObject))
				{
					smi.GoTo(this.blocked_from_growing);
				}
			})
				.ToggleComponent<Growing>();
			this.alive.idle.EventTransition(GameHashes.Wilt, this.alive.wilting, (StandardCropPlant.StatesInstance smi) => smi.master.wiltCondition.IsWilting()).EventTransition(GameHashes.CropDepleted, this.dead, (StandardCropPlant.StatesInstance smi) => !smi.master.crop.CanGrow()).EventTransition(GameHashes.Grow, this.alive.pre_fruiting, (StandardCropPlant.StatesInstance smi) => smi.master.growing.ReachedNextHarvest())
				.PlayAnim("grow", KAnim.PlayMode.Paused, (StandardCropPlant.StatesInstance smi) => (!smi.HasBeenHarvested()) ? string.Empty : "_harvest")
				.Enter(delegate(StandardCropPlant.StatesInstance smi)
				{
					smi.master.animController.SetPositionPercent(smi.master.growing.PercentOfCurrentHarvest());
				})
				.Update(delegate(StandardCropPlant.StatesInstance smi)
				{
					smi.master.animController.SetPositionPercent(smi.master.growing.PercentOfCurrentHarvest());
				});
			this.alive.pre_fruiting.QueueAnim("grow_pst", false, null).EventHandler(GameHashes.AnimQueueComplete, delegate(StandardCropPlant.StatesInstance smi)
			{
				smi.GoTo(this.alive.fruiting);
			});
			this.alive.wilting.PlayAnim("wilt", KAnim.PlayMode.Loop, (StandardCropPlant.StatesInstance smi) => smi.WiltStage().ToString()).EventTransition(GameHashes.WiltRecover, this.alive.idle, (StandardCropPlant.StatesInstance smi) => !smi.master.wiltCondition.IsWilting());
			this.alive.fruiting.DefaultState(this.alive.fruiting.fruiting_idle).EventHandler(GameHashes.Wilt, delegate(StandardCropPlant.StatesInstance smi)
			{
				smi.master.crop.SpawnFruit(null);
				smi.master.harvestable.SetCanBeHarvested(false);
				smi.GoTo(this.alive.wilting);
			}).EventTransition(GameHashes.Harvest, this.alive.fruiting.fruiting_harvest, null);
			this.alive.fruiting.fruiting_idle.PlayAnim("idle_full", KAnim.PlayMode.Loop, null).Enter(delegate(StandardCropPlant.StatesInstance smi)
			{
				smi.master.harvestable.SetCanBeHarvested(true);
			}).Transition(this.alive.fruiting.fruiting_old, (StandardCropPlant.StatesInstance smi) => smi.IsOld());
			this.alive.fruiting.fruiting_old.PlayAnim("wilt", KAnim.PlayMode.Loop, (StandardCropPlant.StatesInstance smi) => smi.WiltStage().ToString()).Enter(delegate(StandardCropPlant.StatesInstance smi)
			{
				smi.master.harvestable.SetCanBeHarvested(true);
			}).Transition(this.alive.fruiting.fruiting_idle, (StandardCropPlant.StatesInstance smi) => !smi.IsOld());
			this.alive.fruiting.fruiting_harvest.PlayAnim("harvest", KAnim.PlayMode.Once, null).Enter(delegate(StandardCropPlant.StatesInstance smi)
			{
				if (GameScheduler.Instance != null && smi.master != null)
				{
					GameScheduler.Instance.Schedule("SpawnFruit", 0.2f, new Action<object>(smi.master.crop.SpawnFruit), null, null);
				}
				smi.master.harvestable.SetCanBeHarvested(false);
			}).OnAnimQueueComplete(this.alive.idle);
		}

		public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State blocked_from_growing;

		public StandardCropPlant.States.AliveStates alive;

		public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State dead;

		public class AliveStates : GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.PlantAliveSubState
		{
			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State idle;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State pre_fruiting;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State barren;

			public StandardCropPlant.States.FruitingState fruiting;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State wilting;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State destroy;
		}

		public class FruitingState : GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State
		{
			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State fruiting_idle;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State fruiting_old;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State fruiting_harvest;
		}
	}
}
