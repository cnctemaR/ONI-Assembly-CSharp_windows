using System;
using UnityEngine;

public class BasicForagePlantPlanted : StateMachineComponent<BasicForagePlantPlanted.StatesInstance>
{
	private static bool DoesNOTHavePreDeathAnimation(BasicForagePlantPlanted.StatesInstance smi, object o)
	{
		return string.IsNullOrEmpty(smi.master.Pre_Death_Anim);
	}

	private static bool HasPreDeathAnimation(BasicForagePlantPlanted.StatesInstance smi, object o)
	{
		return !string.IsNullOrEmpty(smi.master.Pre_Death_Anim);
	}

	private static void DropSeed(BasicForagePlantPlanted.StatesInstance smi)
	{
		smi.master.seedProducer.DropSeed(null);
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

	public string Pre_Death_Anim;

	[MyCmpReq]
	private Harvestable harvestable;

	[MyCmpReq]
	private SeedProducer seedProducer;

	[MyCmpReq]
	private KBatchedAnimController animController;

	public class StatesInstance : GameStateMachine<BasicForagePlantPlanted.States, BasicForagePlantPlanted.StatesInstance, BasicForagePlantPlanted, object>.GameInstance
	{
		public StatesInstance(BasicForagePlantPlanted smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<BasicForagePlantPlanted.States, BasicForagePlantPlanted.StatesInstance, BasicForagePlantPlanted>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.seed_grow;
			base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
			this.seed_grow.PlayAnim("idle", KAnim.PlayMode.Once).EventTransition(GameHashes.AnimQueueComplete, this.alive.idle, null);
			this.alive.InitializeStates(this.masterTarget, this.dead);
			this.alive.idle.PlayAnim("idle").EventHandlerTransition(GameHashes.Harvest, this.alive.harvest, new Func<BasicForagePlantPlanted.StatesInstance, object, bool>(BasicForagePlantPlanted.DoesNOTHavePreDeathAnimation)).EventHandlerTransition(GameHashes.Harvest, this.alive.harvestDelayed, new Func<BasicForagePlantPlanted.StatesInstance, object, bool>(BasicForagePlantPlanted.HasPreDeathAnimation))
				.Enter(delegate(BasicForagePlantPlanted.StatesInstance smi)
				{
					smi.master.harvestable.SetCanBeHarvested(true);
				});
			this.alive.harvestDelayed.PlayAnim((BasicForagePlantPlanted.StatesInstance smi) => smi.master.Pre_Death_Anim, KAnim.PlayMode.Once).OnAnimQueueComplete(this.alive.harvest);
			this.alive.harvest.Enter(new StateMachine<BasicForagePlantPlanted.States, BasicForagePlantPlanted.StatesInstance, BasicForagePlantPlanted, object>.State.Callback(BasicForagePlantPlanted.DropSeed)).EnterGoTo(this.dead);
			this.dead.Enter(delegate(BasicForagePlantPlanted.StatesInstance smi)
			{
				GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.master.transform.GetPosition(), Grid.SceneLayer.FXFront, null, 0).SetActive(true);
				smi.master.Trigger(1623392196, null);
				smi.master.animController.StopAndClear();
				global::UnityEngine.Object.Destroy(smi.master.animController);
				smi.master.DestroySelf(null);
			});
		}

		public GameStateMachine<BasicForagePlantPlanted.States, BasicForagePlantPlanted.StatesInstance, BasicForagePlantPlanted, object>.State seed_grow;

		public BasicForagePlantPlanted.States.AliveStates alive;

		public GameStateMachine<BasicForagePlantPlanted.States, BasicForagePlantPlanted.StatesInstance, BasicForagePlantPlanted, object>.State dead;

		public class AliveStates : GameStateMachine<BasicForagePlantPlanted.States, BasicForagePlantPlanted.StatesInstance, BasicForagePlantPlanted, object>.PlantAliveSubState
		{
			public GameStateMachine<BasicForagePlantPlanted.States, BasicForagePlantPlanted.StatesInstance, BasicForagePlantPlanted, object>.State idle;

			public GameStateMachine<BasicForagePlantPlanted.States, BasicForagePlantPlanted.StatesInstance, BasicForagePlantPlanted, object>.State harvest;

			public GameStateMachine<BasicForagePlantPlanted.States, BasicForagePlantPlanted.StatesInstance, BasicForagePlantPlanted, object>.State harvestDelayed;
		}
	}
}
