using System;
using Klei.AI;
using TUNING;
using UnityEngine;

public class PrickleGrass : StateMachineComponent<PrickleGrass.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe(1309017699, delegate(object o)
		{
			this.replanted = true;
		});
		this.growth_bonus.Description = "Growth Bonus";
		this.wilt_penalty.Description = "Wilt Penalty";
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.animController.randomiseLoopedOffset = true;
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

	public bool replanted = false;

	private AttributeModifier growth_bonus = new AttributeModifier("Effect", (float)DECOR.BONUS.TIER3.amount, null, false, false, true);

	private AttributeModifier wilt_penalty = new AttributeModifier("Effect", (float)DECOR.PENALTY.TIER1.amount, null, false, false, true);

	public class StatesInstance : GameStateMachine<PrickleGrass.States, PrickleGrass.StatesInstance, PrickleGrass, object>.GameInstance
	{
		public StatesInstance(PrickleGrass smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<PrickleGrass.States, PrickleGrass.StatesInstance, PrickleGrass>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.grow;
			base.serializable = true;
			this.dead.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Dead).Enter(delegate(PrickleGrass.StatesInstance smi)
			{
				GameUtil.KInstantiate(EffectPrefabs.Instance.PlantDeath, smi.master.transform.position, Grid.SceneLayer.FXFront, SceneOrganizer.Instance.GetFolder(Folder.FX), null, 0);
				smi.master.Trigger(1623392196, null);
				smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
				global::UnityEngine.Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
				smi.Schedule(0.5f, new Action<object>(smi.master.DestroySelf), null);
			});
			this.blocked_from_growing.ToggleStatusItem(Db.Get().MiscStatusItems.RegionIsBlocked, null).EventTransition(GameHashes.EntombedChanged, this.alive, (PrickleGrass.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject)).EventTransition(GameHashes.TooColdWarning, this.alive, (PrickleGrass.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject))
				.EventTransition(GameHashes.TooHotWarning, this.alive, (PrickleGrass.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject))
				.EventTransition(GameHashes.Uprooted, this.dead, (PrickleGrass.StatesInstance smi) => UprootedMonitor.IsObjectUprooted(smi.master.gameObject));
			this.grow.Enter(delegate(PrickleGrass.StatesInstance smi)
			{
				if (smi.master.replanted && !this.alive.ForceUpdateStatus(smi.master.gameObject))
				{
					smi.GoTo(this.blocked_from_growing);
				}
			}).PlayAnim("grow_seed", KAnim.PlayMode.Once).EventTransition(GameHashes.AnimQueueComplete, this.alive, null);
			this.alive.InitializeStates(this.masterTarget, this.dead).DefaultState(this.alive.idle).ToggleStatusItem(Db.Get().CreatureStatusItems.Idle, null);
			this.alive.idle.EventTransition(GameHashes.Wilt, this.alive.wilting, (PrickleGrass.StatesInstance smi) => smi.master.wiltCondition.IsWilting()).PlayAnim("idle", KAnim.PlayMode.Loop).Enter(delegate(PrickleGrass.StatesInstance smi)
			{
				smi.master.growth_bonus.Description = "Growth Bonus";
				smi.master.GetAttributes().Get(Db.Get().Attributes.Decor).Remove(smi.master.wilt_penalty);
				smi.master.GetAttributes().Get(Db.Get().Attributes.Decor).Add("growth_bonus", smi.master.growth_bonus);
				smi.master.GetComponent<DecorProvider>().Refresh();
			});
			this.alive.wilting.PlayAnim("wilt1", KAnim.PlayMode.Loop).EventTransition(GameHashes.WiltRecover, this.alive.idle, null).Enter(delegate(PrickleGrass.StatesInstance smi)
			{
				smi.master.growth_bonus.Description = "Wilt Penalty";
				smi.master.GetAttributes().Get(Db.Get().Attributes.Decor).Remove(smi.master.growth_bonus);
				smi.master.GetAttributes().Get(Db.Get().Attributes.Decor).Add("wilt_penalty", smi.master.wilt_penalty);
				smi.master.GetComponent<DecorProvider>().SetValues(DECOR.PENALTY.TIER1);
				smi.master.GetComponent<DecorProvider>().Refresh();
			});
		}

		public GameStateMachine<PrickleGrass.States, PrickleGrass.StatesInstance, PrickleGrass, object>.State grow;

		public GameStateMachine<PrickleGrass.States, PrickleGrass.StatesInstance, PrickleGrass, object>.State blocked_from_growing;

		public PrickleGrass.States.AliveStates alive;

		public GameStateMachine<PrickleGrass.States, PrickleGrass.StatesInstance, PrickleGrass, object>.State dead;

		public class AliveStates : GameStateMachine<PrickleGrass.States, PrickleGrass.StatesInstance, PrickleGrass, object>.PlantAliveSubState
		{
			public GameStateMachine<PrickleGrass.States, PrickleGrass.StatesInstance, PrickleGrass, object>.State idle;

			public PrickleGrass.States.WiltingState wilting;
		}

		public class WiltingState : GameStateMachine<PrickleGrass.States, PrickleGrass.StatesInstance, PrickleGrass, object>.State
		{
			public GameStateMachine<PrickleGrass.States, PrickleGrass.StatesInstance, PrickleGrass, object>.State wilting_pre;

			public GameStateMachine<PrickleGrass.States, PrickleGrass.StatesInstance, PrickleGrass, object>.State wilting;

			public GameStateMachine<PrickleGrass.States, PrickleGrass.StatesInstance, PrickleGrass, object>.State wilting_pst;
		}
	}
}
