using System;
using STRINGS;
using UnityEngine;

public class Dinofern : StateMachineComponent<Dinofern.StatesInstance>
{
	protected void DestroySelf(object callbackParam)
	{
		CreatureHelpers.DeselectCreature(base.gameObject);
		Util.KDestroyGameObject(base.gameObject);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public void SetConsumptionRate()
	{
		if (this.receptacleMonitor.Replanted)
		{
			this.elementConsumer.consumptionRate = 0.09f;
			return;
		}
		this.elementConsumer.consumptionRate = 0.0225f;
	}

	[MyCmpReq]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private ElementConsumer elementConsumer;

	[MyCmpReq]
	private ReceptacleMonitor receptacleMonitor;

	private Growing growing;

	public class StatesInstance : GameStateMachine<Dinofern.States, Dinofern.StatesInstance, Dinofern, object>.GameInstance
	{
		public StatesInstance(Dinofern master)
			: base(master)
		{
			master.growing = base.GetComponent<Growing>();
		}
	}

	public class States : GameStateMachine<Dinofern.States, Dinofern.StatesInstance, Dinofern>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
			default_state = this.grow;
			GameStateMachine<Dinofern.States, Dinofern.StatesInstance, Dinofern, object>.State state = this.dead;
			string text = CREATURES.STATUSITEMS.DEAD.NAME;
			string text2 = CREATURES.STATUSITEMS.DEAD.TOOLTIP;
			string text3 = "";
			StatusItem.IconType iconType = StatusItem.IconType.Info;
			NotificationType notificationType = NotificationType.Neutral;
			bool flag = false;
			StatusItemCategory main = Db.Get().StatusItemCategories.Main;
			state.ToggleStatusItem(text, text2, text3, iconType, notificationType, flag, default(HashedString), 129022, null, null, main).Enter(delegate(Dinofern.StatesInstance smi)
			{
				GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.master.transform.GetPosition(), Grid.SceneLayer.FXFront, null, 0).SetActive(true);
				smi.master.Trigger(1623392196, null);
				smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
				global::UnityEngine.Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
				smi.Schedule(0.5f, new Action<object>(smi.master.DestroySelf), null);
			});
			this.blocked_from_growing.ToggleStatusItem(Db.Get().MiscStatusItems.RegionIsBlocked, null).EventTransition(GameHashes.EntombedChanged, this.alive, (Dinofern.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject)).EventTransition(GameHashes.TooColdWarning, this.alive, (Dinofern.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject))
				.EventTransition(GameHashes.TooHotWarning, this.alive, (Dinofern.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject))
				.TagTransition(GameTags.Uprooted, this.dead, false);
			this.grow.Enter(delegate(Dinofern.StatesInstance smi)
			{
				if (smi.master.receptacleMonitor.HasReceptacle() && !this.alive.ForceUpdateStatus(smi.master.gameObject))
				{
					smi.GoTo(this.blocked_from_growing);
				}
			}).EventTransition(GameHashes.AnimQueueComplete, this.alive, null);
			this.alive.InitializeStates(this.masterTarget, this.dead).DefaultState(this.alive.growing);
			this.alive.growing.Transition(this.alive.mature, (Dinofern.StatesInstance smi) => smi.master.growing.IsGrown(), UpdateRate.SIM_200ms).EventTransition(GameHashes.Wilt, this.alive.wilting, (Dinofern.StatesInstance smi) => smi.master.wiltCondition.IsWilting()).Enter(delegate(Dinofern.StatesInstance smi)
			{
				smi.master.elementConsumer.EnableConsumption(true);
			})
				.Exit(delegate(Dinofern.StatesInstance smi)
				{
					smi.master.elementConsumer.EnableConsumption(false);
				});
			this.alive.mature.Transition(this.alive.growing, (Dinofern.StatesInstance smi) => !smi.master.growing.IsGrown(), UpdateRate.SIM_200ms).EventTransition(GameHashes.Wilt, this.alive.wilting, (Dinofern.StatesInstance smi) => smi.master.wiltCondition.IsWilting());
			this.alive.wilting.EventTransition(GameHashes.WiltRecover, this.alive.growing, (Dinofern.StatesInstance smi) => !smi.master.wiltCondition.IsWilting());
		}

		public GameStateMachine<Dinofern.States, Dinofern.StatesInstance, Dinofern, object>.State grow;

		public GameStateMachine<Dinofern.States, Dinofern.StatesInstance, Dinofern, object>.State blocked_from_growing;

		public Dinofern.States.AliveStates alive;

		public GameStateMachine<Dinofern.States, Dinofern.StatesInstance, Dinofern, object>.State dead;

		public class AliveStates : GameStateMachine<Dinofern.States, Dinofern.StatesInstance, Dinofern, object>.PlantAliveSubState
		{
			public GameStateMachine<Dinofern.States, Dinofern.StatesInstance, Dinofern, object>.State growing;

			public GameStateMachine<Dinofern.States, Dinofern.StatesInstance, Dinofern, object>.State mature;

			public GameStateMachine<Dinofern.States, Dinofern.StatesInstance, Dinofern, object>.State wilting;
		}
	}
}
