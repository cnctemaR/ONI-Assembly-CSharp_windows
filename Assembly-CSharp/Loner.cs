using System;

[SkipSaveFileSerialization]
public class Loner : StateMachineComponent<Loner.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	public class StatesInstance : GameStateMachine<Loner.States, Loner.StatesInstance, Loner, object>.GameInstance
	{
		public StatesInstance(Loner master)
			: base(master)
		{
		}

		public bool IsAlone()
		{
			WorldContainer myWorld = this.GetMyWorld();
			if (!myWorld)
			{
				return false;
			}
			int parentWorldId = myWorld.ParentWorldId;
			int id = myWorld.id;
			MinionIdentity component = base.GetComponent<MinionIdentity>();
			foreach (object obj in Components.LiveMinionIdentities)
			{
				MinionIdentity minionIdentity = (MinionIdentity)obj;
				if (component != minionIdentity)
				{
					int myWorldId = minionIdentity.GetMyWorldId();
					if (id == myWorldId || parentWorldId == myWorldId)
					{
						return false;
					}
				}
			}
			return true;
		}
	}

	public class States : GameStateMachine<Loner.States, Loner.StatesInstance, Loner>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.root.Enter(delegate(Loner.StatesInstance smi)
			{
				if (smi.IsAlone())
				{
					smi.GoTo(this.alone);
				}
			});
			this.idle.EventTransition(GameHashes.MinionMigration, (Loner.StatesInstance smi) => Game.Instance, this.alone, (Loner.StatesInstance smi) => smi.IsAlone()).EventTransition(GameHashes.MinionDelta, (Loner.StatesInstance smi) => Game.Instance, this.alone, (Loner.StatesInstance smi) => smi.IsAlone());
			this.alone.EventTransition(GameHashes.MinionMigration, (Loner.StatesInstance smi) => Game.Instance, this.idle, (Loner.StatesInstance smi) => !smi.IsAlone()).EventTransition(GameHashes.MinionDelta, (Loner.StatesInstance smi) => Game.Instance, this.idle, (Loner.StatesInstance smi) => !smi.IsAlone()).ToggleEffect("Loner");
		}

		public GameStateMachine<Loner.States, Loner.StatesInstance, Loner, object>.State idle;

		public GameStateMachine<Loner.States, Loner.StatesInstance, Loner, object>.State alone;
	}
}
