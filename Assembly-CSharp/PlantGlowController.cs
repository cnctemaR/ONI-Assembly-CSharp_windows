using System;

public class PlantGlowController : StateMachineComponent<PlantGlowController.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	public class StatesInstance : GameStateMachine<PlantGlowController.States, PlantGlowController.StatesInstance, PlantGlowController, object>.GameInstance
	{
		public StatesInstance(PlantGlowController master)
			: base(master)
		{
		}

		[MyCmpGet]
		public Light2D light2D;
	}

	public class States : GameStateMachine<PlantGlowController.States, PlantGlowController.StatesInstance, PlantGlowController>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.wilted;
			this.root.TagTransition(GameTags.Dead, this.dead, false);
			this.wilted.EventTransition(GameHashes.WiltRecover, this.emitting, null).Enter(delegate(PlantGlowController.StatesInstance smi)
			{
				smi.light2D.enabled = false;
			});
			this.emitting.EventTransition(GameHashes.Wilt, this.wilted, null).Enter(delegate(PlantGlowController.StatesInstance smi)
			{
				smi.light2D.enabled = true;
			});
			this.dead.DoNothing();
		}

		public GameStateMachine<PlantGlowController.States, PlantGlowController.StatesInstance, PlantGlowController, object>.State emitting;

		public GameStateMachine<PlantGlowController.States, PlantGlowController.StatesInstance, PlantGlowController, object>.State wilted;

		public GameStateMachine<PlantGlowController.States, PlantGlowController.StatesInstance, PlantGlowController, object>.State dead;
	}
}
