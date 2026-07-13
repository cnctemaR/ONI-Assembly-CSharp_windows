using System;
using Klei.AI;

[SkipSaveFileSerialization]
public class StarryEyed : StateMachineComponent<StarryEyed.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	private const string STARRY_EYED_EFFECT_NAME = "StarryEyed";

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
			this.idle.EventTransition(GameHashes.MinionMigration, (StarryEyed.StatesInstance smi) => Game.Instance, this.inSpace, (StarryEyed.StatesInstance smi) => smi.IsInSpace()).Enter(delegate(StarryEyed.StatesInstance smi)
			{
				Effects component = smi.master.gameObject.GetComponent<Effects>();
				if (component != null && component.HasEffect("StarryEyed"))
				{
					component.Remove("StarryEyed");
				}
			});
			this.inSpace.EventTransition(GameHashes.MinionMigration, (StarryEyed.StatesInstance smi) => Game.Instance, this.idle, (StarryEyed.StatesInstance smi) => !smi.IsInSpace()).ToggleEffect("StarryEyed");
		}

		public GameStateMachine<StarryEyed.States, StarryEyed.StatesInstance, StarryEyed, object>.State idle;

		public GameStateMachine<StarryEyed.States, StarryEyed.StatesInstance, StarryEyed, object>.State inSpace;
	}
}
