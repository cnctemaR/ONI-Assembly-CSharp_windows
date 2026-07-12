using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class CritterTrapPlant : StateMachineComponent<CritterTrapPlant.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.master.growing.enabled = false;
		base.Subscribe<CritterTrapPlant>(-216549700, CritterTrapPlant.OnUprootedDelegate);
		base.smi.StartSM();
	}

	public void RefreshPositionPercent()
	{
		this.animController.SetPositionPercent(this.growing.PercentOfCurrentHarvest());
	}

	private void OnUprooted(object data = null)
	{
		GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), base.gameObject.transform.GetPosition(), Grid.SceneLayer.FXFront, null, 0).SetActive(true);
		base.gameObject.Trigger(1623392196, null);
		base.gameObject.GetComponent<KBatchedAnimController>().StopAndClear();
		global::UnityEngine.Object.Destroy(base.gameObject.GetComponent<KBatchedAnimController>());
		Util.KDestroyGameObject(base.gameObject);
	}

	protected void DestroySelf(object callbackParam)
	{
		CreatureHelpers.DeselectCreature(base.gameObject);
		Util.KDestroyGameObject(base.gameObject);
	}

	public Notification CreateDeathNotification()
	{
		return new Notification(CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION, NotificationType.Bad, (List<Notification> notificationList, object data) => CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false), "/t• " + base.gameObject.GetProperName(), true, 0f, null, null, null, true);
	}

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

	[MyCmpReq]
	private Harvestable harvestable;

	[MyCmpReq]
	private Storage storage;

	public float gasOutputRate;

	public float gasVentThreshold;

	public SimHashes outputElement;

	private float GAS_TEMPERATURE_DELTA = 10f;

	private static readonly EventSystem.IntraObjectHandler<CritterTrapPlant> OnUprootedDelegate = new EventSystem.IntraObjectHandler<CritterTrapPlant>(delegate(CritterTrapPlant component, object data)
	{
		component.OnUprooted(data);
	});

	public class StatesInstance : GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.GameInstance
	{
		public StatesInstance(CritterTrapPlant master)
			: base(master)
		{
		}

		public void OnTrapTriggered(object data)
		{
			base.smi.sm.trapTriggered.Trigger(base.smi);
		}

		public void AddGas(float dt)
		{
			float num = base.smi.GetComponent<PrimaryElement>().Temperature + base.smi.master.GAS_TEMPERATURE_DELTA;
			base.smi.master.storage.AddGasChunk(base.smi.master.outputElement, base.smi.master.gasOutputRate * dt, num, byte.MaxValue, 0, false, true);
			if (this.ShouldVentGas())
			{
				base.smi.sm.ventGas.Trigger(base.smi);
			}
		}

		public void VentGas()
		{
			PrimaryElement primaryElement = base.smi.master.storage.FindPrimaryElement(base.smi.master.outputElement);
			if (primaryElement != null)
			{
				SimMessages.AddRemoveSubstance(Grid.PosToCell(base.smi.transform.GetPosition()), primaryElement.ElementID, CellEventLogger.Instance.Dumpable, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount, true, -1);
				base.smi.master.storage.ConsumeIgnoringDisease(primaryElement.gameObject);
			}
		}

		public bool ShouldVentGas()
		{
			return base.smi.master.storage.FindPrimaryElement(base.smi.master.outputElement).Mass >= base.smi.master.gasVentThreshold;
		}
	}

	public class States : GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
			default_state = this.trap;
			this.trap.DefaultState(this.trap.open);
			this.trap.open.ToggleComponent<TrapTrigger>(false).EventHandler(GameHashes.TrapTriggered, delegate(CritterTrapPlant.StatesInstance smi, object data)
			{
				smi.OnTrapTriggered(data);
			}).OnSignal(this.trapTriggered, this.trap.trigger)
				.ParamTransition<bool>(this.hasEatenCreature, this.trap.digesting, GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.IsTrue)
				.PlayAnim("idle_open", KAnim.PlayMode.Loop)
				.EventTransition(GameHashes.Wilt, this.trap.wilting, null);
			this.trap.trigger.PlayAnim("trap", KAnim.PlayMode.Once).Enter(delegate(CritterTrapPlant.StatesInstance smi)
			{
				smi.master.storage.ConsumeAllIgnoringDisease();
				smi.sm.hasEatenCreature.Set(true, smi);
			}).OnAnimQueueComplete(this.trap.digesting);
			this.trap.digesting.PlayAnim("digesting_loop", KAnim.PlayMode.Loop).ToggleComponent<Growing>(false).EventTransition(GameHashes.Grow, this.fruiting.enter, (CritterTrapPlant.StatesInstance smi) => smi.master.growing.ReachedNextHarvest())
				.EventTransition(GameHashes.Wilt, this.trap.wilting, null)
				.DefaultState(this.trap.digesting.idle);
			this.trap.digesting.idle.PlayAnim("digesting_loop", KAnim.PlayMode.Loop).Update(delegate(CritterTrapPlant.StatesInstance smi, float dt)
			{
				smi.AddGas(dt);
			}, UpdateRate.SIM_4000ms, false).OnSignal(this.ventGas, this.trap.digesting.vent_pre);
			this.trap.digesting.vent_pre.PlayAnim("vent_pre").Exit(delegate(CritterTrapPlant.StatesInstance smi)
			{
				smi.VentGas();
			}).OnAnimQueueComplete(this.trap.digesting.vent);
			this.trap.digesting.vent.PlayAnim("vent_loop", KAnim.PlayMode.Once).QueueAnim("vent_pst", false, null).OnAnimQueueComplete(this.trap.digesting.idle);
			this.trap.wilting.PlayAnim("wilt1", KAnim.PlayMode.Loop).EventTransition(GameHashes.WiltRecover, this.trap, (CritterTrapPlant.StatesInstance smi) => !smi.master.wiltCondition.IsWilting());
			this.fruiting.EventTransition(GameHashes.Wilt, this.fruiting.wilting, null).EventTransition(GameHashes.Harvest, this.harvest, null).DefaultState(this.fruiting.idle);
			this.fruiting.enter.PlayAnim("open_harvest", KAnim.PlayMode.Once).Exit(delegate(CritterTrapPlant.StatesInstance smi)
			{
				smi.VentGas();
				smi.master.storage.ConsumeAllIgnoringDisease();
			}).OnAnimQueueComplete(this.fruiting.idle);
			this.fruiting.idle.PlayAnim("harvestable_loop", KAnim.PlayMode.Once).Enter(delegate(CritterTrapPlant.StatesInstance smi)
			{
				if (smi.master.harvestable != null)
				{
					smi.master.harvestable.SetCanBeHarvested(true);
				}
			}).Transition(this.fruiting.old, new StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.Transition.ConditionCallback(this.IsOld), UpdateRate.SIM_4000ms);
			this.fruiting.old.PlayAnim("wilt1", KAnim.PlayMode.Once).Enter(delegate(CritterTrapPlant.StatesInstance smi)
			{
				if (smi.master.harvestable != null)
				{
					smi.master.harvestable.SetCanBeHarvested(true);
				}
			}).Transition(this.fruiting.idle, GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.Not(new StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.Transition.ConditionCallback(this.IsOld)), UpdateRate.SIM_4000ms);
			this.fruiting.wilting.PlayAnim("wilt1", KAnim.PlayMode.Once).EventTransition(GameHashes.WiltRecover, this.fruiting, (CritterTrapPlant.StatesInstance smi) => !smi.master.wiltCondition.IsWilting());
			this.harvest.PlayAnim("harvest", KAnim.PlayMode.Once).Enter(delegate(CritterTrapPlant.StatesInstance smi)
			{
				if (GameScheduler.Instance != null && smi.master != null)
				{
					GameScheduler.Instance.Schedule("SpawnFruit", 0.2f, new Action<object>(smi.master.crop.SpawnConfiguredFruit), null, null);
				}
				smi.master.harvestable.SetCanBeHarvested(false);
			}).Exit(delegate(CritterTrapPlant.StatesInstance smi)
			{
				smi.sm.hasEatenCreature.Set(false, smi);
			})
				.OnAnimQueueComplete(this.trap.open);
			this.dead.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Dead, null).Enter(delegate(CritterTrapPlant.StatesInstance smi)
			{
				if (smi.master.rm.Replanted && !smi.master.GetComponent<KPrefabID>().HasTag(GameTags.Uprooted))
				{
					Notifier notifier = smi.master.gameObject.AddOrGet<Notifier>();
					Notification notification = smi.master.CreateDeathNotification();
					notifier.Add(notification, "");
				}
				GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.master.transform.GetPosition(), Grid.SceneLayer.FXFront, null, 0).SetActive(true);
				Harvestable harvestable = smi.master.harvestable;
				if (harvestable != null && harvestable.CanBeHarvested && GameScheduler.Instance != null)
				{
					GameScheduler.Instance.Schedule("SpawnFruit", 0.2f, new Action<object>(smi.master.crop.SpawnConfiguredFruit), null, null);
				}
				smi.master.Trigger(1623392196, null);
				smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
				global::UnityEngine.Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
				smi.Schedule(0.5f, new Action<object>(smi.master.DestroySelf), null);
			});
		}

		public bool IsOld(CritterTrapPlant.StatesInstance smi)
		{
			return smi.master.growing.PercentOldAge() > 0.5f;
		}

		public StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.Signal trapTriggered;

		public StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.Signal ventGas;

		public StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.BoolParameter hasEatenCreature;

		public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State dead;

		public CritterTrapPlant.States.FruitingStates fruiting;

		public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State harvest;

		public CritterTrapPlant.States.TrapStates trap;

		public class DigestingStates : GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State
		{
			public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State idle;

			public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State vent_pre;

			public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State vent;
		}

		public class TrapStates : GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State
		{
			public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State open;

			public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State trigger;

			public CritterTrapPlant.States.DigestingStates digesting;

			public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State wilting;
		}

		public class FruitingStates : GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State
		{
			public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State enter;

			public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State idle;

			public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State old;

			public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State wilting;
		}
	}
}
