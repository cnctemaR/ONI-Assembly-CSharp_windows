using System;
using Klei.AI;
using STRINGS;

public class LubricatedMovementMonitor : GameStateMachine<LubricatedMovementMonitor, LubricatedMovementMonitor.Instance, IStateMachineTarget, LubricatedMovementMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		this.idle.EnterTransition(this.moving, (LubricatedMovementMonitor.Instance smi) => smi.GetComponent<Navigator>().IsMoving()).EventHandlerTransition(GameHashes.ObjectMovementStateChanged, this.moving, new Func<LubricatedMovementMonitor.Instance, object, bool>(this.IsMoving));
		this.moving.ToggleAttributeModifier("DryingOutVeryFast", (LubricatedMovementMonitor.Instance smi) => smi.movementMoistureModifier, null).EventHandlerTransition(GameHashes.ObjectMovementStateChanged, this.idle, (LubricatedMovementMonitor.Instance smi, object data) => !this.IsMoving(smi, data));
	}

	private bool IsMoving(LubricatedMovementMonitor.Instance smi, object data)
	{
		if (data is GameHashes)
		{
			GameHashes gameHashes = (GameHashes)data;
			return gameHashes == GameHashes.ObjectMovementWakeUp;
		}
		return false;
	}

	public GameStateMachine<LubricatedMovementMonitor, LubricatedMovementMonitor.Instance, IStateMachineTarget, LubricatedMovementMonitor.Def>.State idle;

	public GameStateMachine<LubricatedMovementMonitor, LubricatedMovementMonitor.Instance, IStateMachineTarget, LubricatedMovementMonitor.Def>.State moving;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<LubricatedMovementMonitor, LubricatedMovementMonitor.Instance, IStateMachineTarget, LubricatedMovementMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.moisture = Db.Get().Amounts.Moisture.Lookup(base.gameObject);
			this.movementMoistureModifier = new AttributeModifier(this.moisture.amount.deltaAttribute.Id, this.movingDryRate, CREATURES.MODIFIERS.MOVEMENT_MOISTURE_LOSS.NAME, false, false, true);
		}

		public AmountInstance moisture;

		public AttributeModifier movementMoistureModifier;

		public float movingDryRate = -0.8333333f;
	}
}
