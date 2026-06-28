using System;
using UnityEngine;

public class PrickleGrass : StateMachineComponent<PrickleGrass.StatesInstance>
{
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
			default_state = this.alive;
			base.serializable = true;
			this.dead.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Dead).Enter(delegate(PrickleGrass.StatesInstance smi)
			{
				GameUtil.KInstantiate(EffectPrefabs.Instance.PlantDeath, smi.master.transform.position, Grid.SceneLayer.FXFront, SceneOrganizer.Instance.GetFolder(Folder.FX), null, 0);
				smi.master.Trigger(1623392196, null);
				smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
				global::UnityEngine.Object.Destroy(smi.master.GetComponent<KBatchedAnimController>());
				smi.Schedule(0.5f, new Action<object>(smi.master.DestroySelf), null);
			});
			this.blocked_from_growing.ToggleStatusItem(Db.Get().MiscStatusItems.RegionIsBlocked, null).EventTransition(GameHashes.EntombedChanged, this.dead, (PrickleGrass.StatesInstance smi) => smi.master.entombVulnerable.GetEntombed).EventTransition(GameHashes.TooColdWarning, this.alive.seed_grow, (PrickleGrass.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject))
				.EventTransition(GameHashes.TooHotWarning, this.alive.seed_grow, (PrickleGrass.StatesInstance smi) => this.alive.ForceUpdateStatus(smi.master.gameObject))
				.EventTransition(GameHashes.Uprooted, this.dead, (PrickleGrass.StatesInstance smi) => UprootedMonitor.IsObjectUprooted(smi.master.gameObject));
			this.alive.InitializeStates(this.masterTarget, this.dead).DefaultState(this.alive.seed_grow).Enter(delegate(PrickleGrass.StatesInstance smi)
			{
				if (!this.alive.ForceUpdateStatus(smi.master.gameObject))
				{
					smi.GoTo(this.blocked_from_growing);
				}
			})
				.ToggleStatusItem(Db.Get().CreatureStatusItems.Idle, null);
			this.alive.seed_grow.QueueAnim("grow_seed", false, null).EventTransition(GameHashes.AnimQueueComplete, this.alive.idle, null);
			this.alive.idle.EventTransition(GameHashes.Wilt, this.alive.wilting, (PrickleGrass.StatesInstance smi) => smi.master.wiltCondition.IsWilting()).PlayAnim("idle", KAnim.PlayMode.Loop, null);
			this.alive.wilting.PlayAnim("wilt1", KAnim.PlayMode.Loop, null).EventTransition(GameHashes.WiltRecover, this.alive.idle, null);
		}

		public GameStateMachine<PrickleGrass.States, PrickleGrass.StatesInstance, PrickleGrass, object>.State blocked_from_growing;

		public PrickleGrass.States.AliveStates alive;

		public GameStateMachine<PrickleGrass.States, PrickleGrass.StatesInstance, PrickleGrass, object>.State dead;

		public class AliveStates : GameStateMachine<PrickleGrass.States, PrickleGrass.StatesInstance, PrickleGrass, object>.PlantAliveSubState
		{
			public GameStateMachine<PrickleGrass.States, PrickleGrass.StatesInstance, PrickleGrass, object>.State seed_grow;

			public GameStateMachine<PrickleGrass.States, PrickleGrass.StatesInstance, PrickleGrass, object>.State idle;

			public PrickleGrass.States.WiltingState wilting;

			public GameStateMachine<PrickleGrass.States, PrickleGrass.StatesInstance, PrickleGrass, object>.State destroy;
		}

		public class WiltingState : GameStateMachine<PrickleGrass.States, PrickleGrass.StatesInstance, PrickleGrass, object>.State
		{
			public GameStateMachine<PrickleGrass.States, PrickleGrass.StatesInstance, PrickleGrass, object>.State wilting_pre;

			public GameStateMachine<PrickleGrass.States, PrickleGrass.StatesInstance, PrickleGrass, object>.State wilting;

			public GameStateMachine<PrickleGrass.States, PrickleGrass.StatesInstance, PrickleGrass, object>.State wilting_pst;
		}
	}
}
