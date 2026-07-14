using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class StandardCropPlant : StateMachineComponent<StandardCropPlant.StatesInstance>
{
	public static string GetWiltAnimFromAnimSet(StandardCropPlant.AnimSet set, float growingPercentage)
	{
		return StandardCropPlant.GetGenericWiltAnimFromAnimSet(set, (int stg) => set.GetWiltLevel(stg), growingPercentage);
	}

	public static string GetWiltRecoverAnimFromAnimSet(StandardCropPlant.AnimSet set, float growingPercentage)
	{
		return StandardCropPlant.GetGenericWiltAnimFromAnimSet(set, (int stg) => set.GetWiltRecoverLevel(stg), growingPercentage);
	}

	private static string GetGenericWiltAnimFromAnimSet(StandardCropPlant.AnimSet set, Func<int, string> wiltLevelFn, float growingPercentage)
	{
		int num;
		if (growingPercentage < 0.75f)
		{
			num = 1;
		}
		else if (growingPercentage < 1f)
		{
			num = 2;
		}
		else
		{
			num = 3;
		}
		return wiltLevelFn(num);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	protected void DestroySelf(object callbackParam)
	{
		CreatureHelpers.DeselectCreature(base.gameObject);
		Util.KDestroyGameObject(base.gameObject);
	}

	public Notification CreateDeathNotification()
	{
		return new Notification(CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION, NotificationType.Bad, (List<Notification> notificationList, object data) => CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false), "/t• " + base.gameObject.GetProperName(), true, 0f, null, null, null, true, false, false);
	}

	public void RefreshPositionPercent()
	{
		this.animController.SetPositionPercent(this.growing.PercentOfCurrentHarvest());
	}

	private static string ToolTipResolver(List<Notification> notificationList, object data)
	{
		string text = "";
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

	private const int WILT_LEVELS = 3;

	[MyCmpReq]
	private Crop crop;

	[MyCmpReq]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private ReceptacleMonitor rm;

	[MyCmpReq]
	private Growing growing;

	[MyCmpReq]
	private KAnimControllerBase animController;

	[MyCmpGet]
	private Harvestable harvestable;

	public bool wiltsOnReadyToHarvest;

	public bool preventGrowPositionUpdate;

	public static StandardCropPlant.AnimSet defaultAnimSet = new StandardCropPlant.AnimSet
	{
		pre_grow = null,
		grow = "grow",
		grow_pst = "grow_pst",
		idle_full = "idle_full",
		wilt_base = "wilt",
		wilt_recover_base = null,
		harvest = "harvest",
		waning = "waning"
	};

	public StandardCropPlant.AnimSet anims = StandardCropPlant.defaultAnimSet;

	public class AnimSet
	{
		public void ClearWiltLevelCache()
		{
			this.m_wilt.Clear();
		}

		public string GetWiltLevel(int level)
		{
			return this.GetWiltAnimLevel(this.wilt_base, level);
		}

		public string GetWiltRecoverLevel(int level)
		{
			return this.GetWiltAnimLevel(this.wilt_recover_base, level);
		}

		private string GetWiltAnimLevel(string baseSTR, int level)
		{
			if (baseSTR == null)
			{
				return null;
			}
			if (!this.m_wilt.ContainsKey(baseSTR))
			{
				this.m_wilt[baseSTR] = new string[3];
				for (int i = 0; i < 3; i++)
				{
					this.m_wilt[baseSTR][i] = baseSTR + (i + 1).ToString();
				}
			}
			return this.m_wilt[baseSTR][level - 1];
		}

		public AnimSet()
		{
		}

		public AnimSet(StandardCropPlant.AnimSet template)
		{
			this.pre_grow = template.pre_grow;
			this.grow = template.grow;
			this.grow_pst = template.grow_pst;
			this.idle_full = template.idle_full;
			this.wilt_base = template.wilt_base;
			this.wilt_recover_base = template.wilt_recover_base;
			this.harvest = template.harvest;
			this.waning = template.waning;
			this.grow_playmode = template.grow_playmode;
		}

		public string pre_grow;

		public string grow;

		public string grow_pst;

		public string idle_full;

		public string wilt_base;

		public string wilt_recover_base;

		public string harvest;

		public string waning;

		public KAnim.PlayMode grow_playmode = KAnim.PlayMode.Paused;

		private Dictionary<string, string[]> m_wilt = new Dictionary<string, string[]>();
	}

	public class States : GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
			default_state = this.alive;
			this.dead.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Dead, null).Enter(delegate(StandardCropPlant.StatesInstance smi)
			{
				if (smi.master.rm.Replanted && !smi.master.GetComponent<KPrefabID>().HasTag(GameTags.Uprooted))
				{
					Notifier notifier = smi.master.gameObject.AddOrGet<Notifier>();
					Notification notification = smi.master.CreateDeathNotification();
					notifier.Add(notification, "");
				}
				GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.master.transform.GetPosition(), Grid.SceneLayer.FXFront, null, 0).SetActive(true);
				Harvestable component = smi.master.GetComponent<Harvestable>();
				if (component != null && component.CanBeHarvested && GameScheduler.Instance != null)
				{
					GameScheduler.Instance.Schedule("SpawnFruit", 0.2f, new Action<object>(smi.master.crop.SpawnConfiguredFruit), null, null);
				}
				smi.master.Trigger(1623392196, null);
				smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
				global::UnityEngine.Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
				smi.Schedule(0.5f, new Action<object>(smi.master.DestroySelf), null);
			});
			this.blighted.InitializeStates(this.masterTarget, this.dead).PlayAnim((StandardCropPlant.StatesInstance smi) => smi.master.anims.waning, KAnim.PlayMode.Once).ToggleMainStatusItem(Db.Get().CreatureStatusItems.Crop_Blighted, null)
				.TagTransition(GameTags.Blighted, this.alive, true);
			this.alive.InitializeStates(this.masterTarget, this.dead).DefaultState(this.alive.pre_idle).ToggleComponent<Growing>(false)
				.TagTransition(GameTags.Blighted, this.blighted, false);
			this.alive.pre_idle.EnterTransition(this.alive.idle, (StandardCropPlant.StatesInstance smi) => smi.master.anims.pre_grow == null).PlayAnim((StandardCropPlant.StatesInstance smi) => smi.master.anims.pre_grow, KAnim.PlayMode.Once).OnAnimQueueComplete(this.alive.idle)
				.ScheduleGoTo(8f, this.alive.idle);
			this.alive.idle.EventTransition(GameHashes.Wilt, this.alive.wilting, (StandardCropPlant.StatesInstance smi) => smi.master.wiltCondition.IsWilting()).EventTransition(GameHashes.Grow, this.alive.pre_fruiting, (StandardCropPlant.StatesInstance smi) => smi.master.growing.ReachedNextHarvest()).PlayAnim((StandardCropPlant.StatesInstance smi) => smi.master.anims.grow, (StandardCropPlant.StatesInstance smi) => smi.master.anims.grow_playmode)
				.Enter(new StateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State.Callback(StandardCropPlant.States.RefreshPositionPercent))
				.Update(new Action<StandardCropPlant.StatesInstance, float>(StandardCropPlant.States.RefreshPositionPercent), UpdateRate.SIM_4000ms, false)
				.EventHandler(GameHashes.ConsumePlant, new StateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State.Callback(StandardCropPlant.States.RefreshPositionPercent));
			this.alive.pre_fruiting.PlayAnim((StandardCropPlant.StatesInstance smi) => smi.master.anims.grow_pst, KAnim.PlayMode.Once).TriggerOnEnter(GameHashes.BurstEmitDisease, null).EventTransition(GameHashes.AnimQueueComplete, this.alive.fruiting, null)
				.EventTransition(GameHashes.Wilt, this.alive.wilting, null)
				.ScheduleGoTo(8f, this.alive.fruiting);
			this.alive.fruiting_lost.Enter(delegate(StandardCropPlant.StatesInstance smi)
			{
				if (smi.master.harvestable != null)
				{
					smi.master.harvestable.SetCanBeHarvested(false);
				}
			}).GoTo(this.alive.idle);
			this.alive.wilting.PlayAnim(new Func<StandardCropPlant.StatesInstance, string>(StandardCropPlant.States.GetWiltAnim), KAnim.PlayMode.Once).EventTransition(GameHashes.WiltRecover, this.alive.wiltRecover, (StandardCropPlant.StatesInstance smi) => !smi.master.wiltCondition.IsWilting()).EventTransition(GameHashes.Harvest, this.alive.harvest, null);
			this.alive.wiltRecover.EnterTransition(this.alive.idle, new StateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.Transition.ConditionCallback(StandardCropPlant.States.DoesNotHaveWiltRecoverAnim)).PlayAnim(new Func<StandardCropPlant.StatesInstance, string>(StandardCropPlant.States.GetWiltRecoverAnim), KAnim.PlayMode.Once).OnAnimQueueComplete(this.alive.idle);
			this.alive.fruiting.PlayAnim((StandardCropPlant.StatesInstance smi) => smi.master.anims.idle_full, KAnim.PlayMode.Loop).ToggleTag(GameTags.FullyGrown).Enter(delegate(StandardCropPlant.StatesInstance smi)
			{
				if (smi.master.harvestable != null)
				{
					smi.master.harvestable.SetCanBeHarvested(true);
				}
			})
				.EventHandlerTransition(GameHashes.Wilt, this.alive.wilting, (StandardCropPlant.StatesInstance smi, object obj) => smi.master.wiltsOnReadyToHarvest)
				.EventTransition(GameHashes.Harvest, this.alive.harvest, null)
				.EventTransition(GameHashes.Grow, this.alive.fruiting_lost, (StandardCropPlant.StatesInstance smi) => !smi.master.growing.ReachedNextHarvest());
			this.alive.harvest.PlayAnim((StandardCropPlant.StatesInstance smi) => smi.master.anims.harvest, KAnim.PlayMode.Once).Enter(delegate(StandardCropPlant.StatesInstance smi)
			{
				if (smi.master != null)
				{
					smi.master.crop.SpawnConfiguredFruit(null);
				}
				if (smi.master.harvestable != null)
				{
					smi.master.harvestable.SetCanBeHarvested(false);
				}
			}).Exit(delegate(StandardCropPlant.StatesInstance smi)
			{
				smi.Trigger(113170146, null);
			})
				.OnAnimQueueComplete(this.alive.idle);
		}

		private static string GetWiltAnim(StandardCropPlant.StatesInstance smi)
		{
			float num = smi.master.growing.PercentOfCurrentHarvest();
			return StandardCropPlant.GetWiltAnimFromAnimSet(smi.master.anims, num);
		}

		private static bool DoesNotHaveWiltRecoverAnim(StandardCropPlant.StatesInstance smi)
		{
			return StandardCropPlant.States.GetWiltRecoverAnim(smi) == null;
		}

		private static string GetWiltRecoverAnim(StandardCropPlant.StatesInstance smi)
		{
			float num = smi.master.growing.PercentOfCurrentHarvest();
			return StandardCropPlant.GetWiltRecoverAnimFromAnimSet(smi.master.anims, num);
		}

		private static void RefreshPositionPercent(StandardCropPlant.StatesInstance smi, float dt)
		{
			StandardCropPlant.States.RefreshPositionPercent(smi);
		}

		private static void RefreshPositionPercent(StandardCropPlant.StatesInstance smi)
		{
			if (smi.master.preventGrowPositionUpdate)
			{
				return;
			}
			smi.master.RefreshPositionPercent();
		}

		public StandardCropPlant.States.AliveStates alive;

		public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State dead;

		public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.PlantAliveSubState blighted;

		public class AliveStates : GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.PlantAliveSubState
		{
			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State pre_idle;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State idle;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State pre_fruiting;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State fruiting_lost;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State barren;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State fruiting;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State wilting;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State wiltRecover;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State destroy;

			public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State harvest;
		}
	}

	public class StatesInstance : GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.GameInstance
	{
		public StatesInstance(StandardCropPlant master)
			: base(master)
		{
		}
	}
}
