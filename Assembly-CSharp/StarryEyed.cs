using System;

[SkipSaveFileSerialization]
public class StarryEyed : StateMachineComponent<StarryEyed.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	public class StatesInstance : GameStateMachine<StarryEyed.States, StarryEyed.StatesInstance, StarryEyed, object>.GameInstance
	{
		public StatesInstance(StarryEyed master)
			: base(master)
		{
		}

		public bool IsInSpace()
		{
			WorldContainer myWorld = this.GetMyWorld();
			if (!myWorld)
			{
				return false;
			}
			int parentWorldId = myWorld.ParentWorldId;
			int id = myWorld.id;
			return myWorld.GetComponent<Clustercraft>() && parentWorldId == id;
		}
	}

	public class States : GameStateMachine<StarryEyed.States, StarryEyed.StatesInstance, StarryEyed>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.root.Enter(delegate(StarryEyed.StatesInstance smi)
			{
				if (smi.IsInSpace())
				{
					smi.GoTo(this.inSpace);
				}
			});
			this.idle.EventTransition(GameHashes.MinionMigration, (StarryEyed.StatesInstance smi) => Game.Instance, this.inSpace, (StarryEyed.StatesInstance smi) => smi.IsInSpace());
			this.inSpace.EventTransition(GameHashes.MinionMigration, (StarryEyed.StatesInstance smi) => Game.Instance, this.idle, (StarryEyed.StatesInstance smi) => !smi.IsInSpace()).ToggleEffect("StarryEyed");
		}

		public GameStateMachine<StarryEyed.States, StarryEyed.StatesInstance, StarryEyed, object>.State idle;

		public GameStateMachine<StarryEyed.States, StarryEyed.StatesInstance, StarryEyed, object>.State inSpace;
	}
}
