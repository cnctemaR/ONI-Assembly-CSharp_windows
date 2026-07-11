using System;

[SkipSaveFileSerialization]
public class Fashionable : StateMachineComponent<Fashionable.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	protected bool IsUncomfortable()
	{
		ClothingWearer component = base.GetComponent<ClothingWearer>();
		return component != null && component.currentClothing.decorMod <= 0;
	}

	public class StatesInstance : GameStateMachine<Fashionable.States, Fashionable.StatesInstance, Fashionable, object>.GameInstance
	{
		public StatesInstance(Fashionable master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<Fashionable.States, Fashionable.StatesInstance, Fashionable>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.satisfied;
			this.root.EventHandler(GameHashes.EquippedItemEquipper, delegate(Fashionable.StatesInstance smi)
			{
				if (smi.master.IsUncomfortable())
				{
					smi.GoTo(this.suffering);
					return;
				}
				smi.GoTo(this.satisfied);
			}).EventHandler(GameHashes.UnequippedItemEquipper, delegate(Fashionable.StatesInstance smi)
			{
				if (smi.master.IsUncomfortable())
				{
					smi.GoTo(this.suffering);
					return;
				}
				smi.GoTo(this.satisfied);
			});
			this.suffering.AddEffect("UnfashionableClothing").ToggleExpression(Db.Get().Expressions.Uncomfortable, null);
			this.satisfied.DoNothing();
		}

		public GameStateMachine<Fashionable.States, Fashionable.StatesInstance, Fashionable, object>.State satisfied;

		public GameStateMachine<Fashionable.States, Fashionable.StatesInstance, Fashionable, object>.State suffering;
	}
}
