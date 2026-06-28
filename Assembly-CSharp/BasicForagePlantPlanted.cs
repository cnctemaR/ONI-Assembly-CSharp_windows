using System;
using UnityEngine;

public class BasicForagePlantPlanted : StateMachineComponent<BasicForagePlantPlanted.StatesInstance>
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
			base.serializable = true;
			this.seed_grow.PlayAnim("idle", KAnim.PlayMode.Once, null).EventTransition(GameHashes.AnimQueueComplete, this.alive.idle, null);
			this.alive.InitializeStates(this.masterTarget, this.dead);
			this.alive.idle.PlayAnim("idle", KAnim.PlayMode.Once, null).EventTransition(GameHashes.Uprooted, this.alive.harvest, null).EventTransition(GameHashes.Harvest, this.alive.harvest, null)
				.Enter(delegate(BasicForagePlantPlanted.StatesInstance smi)
				{
					smi.master.harvestable.SetCanBeHarvested(true);
				});
			this.alive.harvest.Enter(delegate(BasicForagePlantPlanted.StatesInstance smi)
			{
				smi.master.seedProducer.DropSeed(null);
			}).GoTo(this.dead);
			this.dead.Enter(delegate(BasicForagePlantPlanted.StatesInstance smi)
			{
				GameUtil.KInstantiate(EffectPrefabs.Instance.PlantDeath, smi.master.transform.position, Grid.SceneLayer.FXFront, SceneOrganizer.Instance.GetFolder(Folder.FX), null, 0);
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
		}
	}
}
