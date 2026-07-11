using System;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class EvilFlower : StateMachineComponent<EvilFlower.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<EvilFlower>(1309017699, EvilFlower.SetReplantedTrueDelegate);
		this.growth_bonus.Description = global::STRINGS.CREATURES.SPECIES.EVILFLOWER.GROWTH_BONUS;
		this.wilt_penalty.Description = global::STRINGS.CREATURES.SPECIES.EVILFLOWER.WILT_PENALTY;
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

	[MyCmpReq]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private EntombVulnerable entombVulnerable;

	public bool replanted;

	private AttributeModifier growth_bonus = new AttributeModifier("Effect", (float)DECOR.BONUS.TIER3.amount, null, false, false, true);

	private AttributeModifier wilt_penalty = new AttributeModifier("Effect", (float)DECOR.PENALTY.TIER1.amount, null, false, false, true);

	public EffectorValues positive_decor_effect = new EffectorValues
	{
		amount = 1,
		radius = 5
	};

	private static readonly EventSystem.IntraObjectHandler<EvilFlower> SetReplantedTrueDelegate = new EventSystem.IntraObjectHandler<EvilFlower>(delegate(EvilFlower component, object data)
	{
		component.replanted = true;
	});

	public class StatesInstance : GameStateMachine<EvilFlower.States, EvilFlower.StatesInstance, EvilFlower, object>.GameInstance
	{
		public StatesInstance(EvilFlower smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<EvilFlower.States, EvilFlower.StatesInstance, EvilFlower>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.grow;
			base.serializable = true;
			GameStateMachine<EvilFlower.States, EvilFlower.StatesInstance, EvilFlower, object>.State state = this.dead;
			string text = global::STRINGS.CREATURES.STATUSITEMS.DEAD.NAME;
			string text2 = global::STRINGS.CREATURES.STATUSITEMS.DEAD.TOOLTIP;
			StatusItemCategory statusItemCategory = Db.Get().StatusItemCategories.Main;
			state.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, statusItemCategory).TriggerOnEnter(GameHashes.BurstEmitDisease, null).ToggleTag(GameTags.PreventEmittingDisease)
				.Enter(delegate(EvilFlower.StatesInstance smi)
				{
					GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.master.transform.GetPosition(), Grid.SceneLayer.FXFront, null, 0).SetActive(true);
					smi.master.Trigger(1623392196, null);
					smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
					global::UnityEngine.Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
					smi.Schedule(0.5f, new Action<object>(smi.master.DestroySelf), null);
				});
			this.blocked_from_growing.ToggleStatusItem(Db.Get().MiscStatusItems.RegionIsBlocked, null).EventTransition(GameHashes.EntombedChanged, this.alive, (EvilFlower.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject)).EventTransition(GameHashes.TooColdWarning, this.alive, (EvilFlower.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject))
				.EventTransition(GameHashes.TooHotWarning, this.alive, (EvilFlower.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject))
				.EventTransition(GameHashes.Uprooted, this.dead, (EvilFlower.StatesInstance smi) => UprootedMonitor.IsObjectUprooted(smi.master.gameObject));
			this.grow.Enter(delegate(EvilFlower.StatesInstance smi)
			{
				if (smi.master.replanted && !this.alive.ForceUpdateStatus(smi.master.gameObject))
				{
					smi.GoTo(this.blocked_from_growing);
				}
			}).PlayAnim("grow_seed", KAnim.PlayMode.Once).EventTransition(GameHashes.AnimQueueComplete, this.alive, null);
			GameStateMachine<EvilFlower.States, EvilFlower.StatesInstance, EvilFlower, object>.State state2 = this.alive.InitializeStates(this.masterTarget, this.dead).DefaultState(this.alive.idle);
			text2 = global::STRINGS.CREATURES.STATUSITEMS.IDLE.NAME;
			text = global::STRINGS.CREATURES.STATUSITEMS.IDLE.TOOLTIP;
			statusItemCategory = Db.Get().StatusItemCategories.Main;
			state2.ToggleStatusItem(text2, text, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, statusItemCategory);
			this.alive.idle.EventTransition(GameHashes.Wilt, this.alive.wilting, (EvilFlower.StatesInstance smi) => smi.master.wiltCondition.IsWilting()).PlayAnim("idle", KAnim.PlayMode.Loop).Enter(delegate(EvilFlower.StatesInstance smi)
			{
				smi.master.growth_bonus.Description = global::STRINGS.CREATURES.SPECIES.EVILFLOWER.GROWTH_BONUS;
				smi.master.GetAttributes().Get(Db.Get().Attributes.Decor).Remove(smi.master.wilt_penalty);
				smi.master.GetAttributes().Get(Db.Get().Attributes.Decor).Add(smi.master.growth_bonus);
				smi.master.GetComponent<DecorProvider>().SetValues(smi.master.positive_decor_effect);
				smi.master.GetComponent<DecorProvider>().Refresh();
				smi.master.AddTag(GameTags.Decoration);
			});
			this.alive.wilting.PlayAnim("wilt1", KAnim.PlayMode.Loop).EventTransition(GameHashes.WiltRecover, this.alive.idle, null).ToggleTag(GameTags.PreventEmittingDisease)
				.Enter(delegate(EvilFlower.StatesInstance smi)
				{
					smi.master.growth_bonus.Description = global::STRINGS.CREATURES.SPECIES.EVILFLOWER.WILT_PENALTY;
					smi.master.GetAttributes().Get(Db.Get().Attributes.Decor).Remove(smi.master.growth_bonus);
					smi.master.GetAttributes().Get(Db.Get().Attributes.Decor).Add(smi.master.wilt_penalty);
					smi.master.GetComponent<DecorProvider>().SetValues(DECOR.PENALTY.TIER1);
					smi.master.GetComponent<DecorProvider>().Refresh();
					smi.master.RemoveTag(GameTags.Decoration);
				});
		}

		public GameStateMachine<EvilFlower.States, EvilFlower.StatesInstance, EvilFlower, object>.State grow;

		public GameStateMachine<EvilFlower.States, EvilFlower.StatesInstance, EvilFlower, object>.State blocked_from_growing;

		public EvilFlower.States.AliveStates alive;

		public GameStateMachine<EvilFlower.States, EvilFlower.StatesInstance, EvilFlower, object>.State dead;

		public class AliveStates : GameStateMachine<EvilFlower.States, EvilFlower.StatesInstance, EvilFlower, object>.PlantAliveSubState
		{
			public GameStateMachine<EvilFlower.States, EvilFlower.StatesInstance, EvilFlower, object>.State idle;

			public EvilFlower.States.WiltingState wilting;
		}

		public class WiltingState : GameStateMachine<EvilFlower.States, EvilFlower.StatesInstance, EvilFlower, object>.State
		{
			public GameStateMachine<EvilFlower.States, EvilFlower.StatesInstance, EvilFlower, object>.State wilting_pre;

			public GameStateMachine<EvilFlower.States, EvilFlower.StatesInstance, EvilFlower, object>.State wilting;

			public GameStateMachine<EvilFlower.States, EvilFlower.StatesInstance, EvilFlower, object>.State wilting_pst;
		}
	}
}
