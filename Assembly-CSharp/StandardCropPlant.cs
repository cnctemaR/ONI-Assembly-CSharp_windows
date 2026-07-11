using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class StandardCropPlant : StateMachineComponent<StandardCropPlant.StatesInstance>
{
	private static void RefreshPositionPercent(StandardCropPlant.StatesInstance smi, float dt)
	{
		StandardCropPlant.RefreshPositionPercent(smi);
	}

	private static void RefreshPositionPercent(StandardCropPlant.StatesInstance smi)
	{
		smi.master.animController.SetPositionPercent(smi.master.growing.PercentOfCurrentHarvest());
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.Get<KBatchedAnimController>().randomiseLoopedOffset = true;
		base.smi.StartSM();
	}

	protected void DestroySelf(object callbackParam)
	{
		CreatureHelpers.DeselectCreature(base.gameObject);
		Util.KDestroyGameObject(base.gameObject);
	}

	public Notification CreateDeathNotification()
	{
		return new Notification(CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION, NotificationType.Bad, HashedString.Invalid, (List<Notification> notificationList, object data) => CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false), "/t• " + base.gameObject.GetProperName(), true, 0f, null, null);
	}

	private static string ToolTipResolver(List<Notification> notificationList, object data)
	{
		string text = string.Empty;
		for (int i = 0; i < notificationList.Count; i++)
		{
			Notification notification = notificationList[i];
			text += (string)notification.tooltipData;
			if (i < notificationList.Count - 1)
			{
				text += "\n";
			}
		}
		return string.Format(CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION_TOOLTIP, text);
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

		public bool IsSleeping()
		{
			CropSleepingMonitor.Instance smi = base.master.GetSMI<CropSleepingMonitor.Instance>();
			return smi != null && smi.IsSleeping();
		}
	}

	public class States : GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = true;
			default_state = this.alive;
			GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State state = this.dead;
			string text = CREATURES.STATUSITEMS.DEAD.NAME;
			string text2 = CREATURES.STATUSITEMS.DEAD.TOOLTIP;
			StatusItemCategory main = Db.Get().StatusItemCategories.Main;
			state.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, main).Enter(delegate(StandardCropPlant.StatesInstance smi)
			{
				if (smi.master.growing.Replanted && !UprootedMonitor.IsObjectUprooted(this.masterTarget.Get(smi)))
				{
					Notifier notifier = smi.master.gameObject.AddOrGet<Notifier>();
					Notification notification = smi.master.CreateDeathNotification();
					notifier.Add(notification, string.Empty);
				}
				GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.master.transform.GetPosition(), Grid.SceneLayer.FXFront, null, 0).SetActive(true);
				smi.master.Trigger(1623392196, null);
				smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
				global::UnityEngine.Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
				smi.Schedule(0.5f, new Action<object>(smi.master.DestroySelf), null);
			});
			this.alive.InitializeStates(this.masterTarget, this.dead).DefaultState(this.alive.idle).ToggleComponent<Growing>();
			this.alive.idle.EventTransition(GameHashes.Wilt, this.alive.wilting, (StandardCropPlant.StatesInstance smi) => smi.master.wiltCondition.IsWilting()).EventTransition(GameHashes.Grow, this.alive.pre_fruiting, (StandardCropPlant.StatesInstance smi) => smi.master.growing.ReachedNextHarvest()).EventTransition(GameHashes.CropSleep, this.alive.sleeping, (StandardCropPlant.StatesInstance smi) => smi.IsSleeping())
				.PlayAnim("grow", KAnim.PlayMode.Paused)
				.Enter(new StateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State.Callback(StandardCropPlant.RefreshPositionPercent))
				.Update(new Action<StandardCropPlant.StatesInstance, float>(StandardCropPlant.RefreshPositionPercent), UpdateRate.SIM_4000ms, false)
				.EventHandler(GameHashes.ConsumePlant, new StateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State.Callback(StandardCropPlant.RefreshPositionPercent));
			this.alive.pre_fruiting.PlayAnim("grow_pst", KAnim.PlayMode.Once).EventTransition(GameHashes.AnimQueueComplete, this.alive.fruiting, null);
			this.alive.fruiting_lost.Enter(delegate(StandardCropPlant.StatesInstance smi)
			{
				smi.master.harvestable.SetCanBeHarvested(false);
			}).GoTo(this.alive.idle);
			this.alive.wilting.PlayAnim("wilt", KAnim.PlayMode.Loop, (StandardCropPlant.StatesInstance smi) => smi.WiltStage().ToString()).EventTransition(GameHashes.WiltRecover, this.alive.idle, (StandardCropPlant.StatesInstance smi) => !smi.master.wiltCondition.IsWilting()).EventTransition(GameHashes.Harvest, this.alive.harvest, null);
			this.alive.sleeping.PlayAnim("grow").EventTransition(GameHashes.CropWakeUp, this.alive.idle, (StandardCropPlant.StatesInstance smi) => !smi.IsSleeping()).EventTransition(GameHashes.Harvest, this.alive.harvest, null)
				.EventTransition(GameHashes.Wilt, this.alive.wilting, null);
			this.alive.fruiting.DefaultState(this.alive.fruiting.fruiting_idle).EventTransition(GameHashes.Wilt, this.alive.wilting, null).EventTransition(GameHashes.Harvest, this.alive.harvest, null)
				.EventTransition(GameHashes.Grow, this.alive.fruiting_lost, (StandardCropPlant.StatesInstance smi) => !smi.master.growing.ReachedNextHarvest());
			this.alive.fruiting.fruiting_idle.PlayAnim("idle_full", KAnim.PlayMode.Loop).Enter(delegate(StandardCropPlant.StatesInstance smi)
			{
				smi.master.harvestable.SetCanBeHarvested(true);
			}).Update("fruiting_idle", delegate(StandardCropPlant.StatesInstance smi, float dt)
			{
				if (smi.IsOld())
				{
					smi.GoTo(this.alive.fruiting.fruiting_old);
				}
			}, UpdateRate.SIM_4000ms, false);
			this.alive.fruiting.fruiting_old.PlayAnim("wilt", KAnim.PlayMode.Loop, (StandardCropPlant.StatesInstance smi) => smi.WiltStage().ToString()).Enter(delegate(StandardCropPlant.StatesInstance smi)
			{
				smi.master.harvestable.SetCanBeHarvested(true);
			}).Update("fruiting_old", delegate(StandardCropPlant.StatesInstance smi, float dt)
			{
				if (!smi.IsOld())
				{
					smi.GoTo(this.alive.fruiting.fruiting_idle);
				}
			}, UpdateRate.SIM_4000ms, false);
			this.alive.harvest.PlayAnim("harvest", KAnim.PlayMode.Once).Enter(delegate(StandardCropPlant.StatesInstance smi)
			{
				if (GameScheduler.Instance != null && smi.master != null)
				{
					GameScheduler.Instance.Schedule("SpawnFruit", 0.2f, new Action<object>(smi.master.crop.SpawnFruit), null, null);
				}
				smi.master.harvestable.SetCanBeHarvested(false);
			}).OnAnimQueueComplete(this.alive.idle);
		}

		public StandardCropPlant.States.AliveStates alive;

		public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State dead;

		public class AliveStates : GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.PlantAliveSubState
		{
			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State idle;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State pre_fruiting;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State fruiting_lost;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State barren;

			public StandardCropPlant.States.FruitingState fruiting;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State wilting;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State destroy;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State harvest;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State sleeping;
		}

		public class FruitingState : GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State
		{
			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State fruiting_idle;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State fruiting_old;
		}
	}
}
