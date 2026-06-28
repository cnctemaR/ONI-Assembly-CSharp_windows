using System;

public class DeliverFoodChore : Chore<DeliverFoodChore.StatesInstance>
{
	public DeliverFoodChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.DeliverFood, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.basic, 0, false, true, 0, null)
	{
		this.smi = new DeliverFoodChore.StatesInstance(this);
		base.AddPrecondition(ChorePreconditions.instance.IsChattable, target);
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		this.smi.sm.requestedrationcount.Set(this.smi.GetComponent<StateMachineController>().GetSMI<RationMonitor.Instance>().GetRationsRemaining(), this.smi);
		this.smi.sm.ediblesource.Set(context.consumerState.gameObject.GetComponent<Sensors>().GetSensor<ClosestEdibleSensor>().GetEdible(), this.smi);
		this.smi.sm.deliverypoint.Set(this.gameObject, this.smi);
		this.smi.sm.deliverer.Set(context.consumerState.gameObject, this.smi);
		base.Begin(context);
	}

	public class StatesInstance : GameStateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.GameInstance
	{
		public StatesInstance(DeliverFoodChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.fetch;
			this.fetch.InitializeStates(this.deliverer, this.ediblesource, this.ediblechunk, this.requestedrationcount, this.actualrationcount, this.movetodeliverypoint, null);
			this.movetodeliverypoint.InitializeStates(this.deliverer, this.deliverypoint, this.drop, null, null, null);
			this.drop.InitializeStates(this.deliverer, this.ediblechunk, this.deliverypoint, this.success, null);
			this.success.ReturnSuccess();
		}

		public StateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.TargetParameter deliverer;

		public StateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.TargetParameter ediblesource;

		public StateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.TargetParameter ediblechunk;

		public StateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.TargetParameter deliverypoint;

		public StateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.FloatParameter requestedrationcount;

		public StateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.FloatParameter actualrationcount;

		public GameStateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.FetchSubState fetch;

		public GameStateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.ApproachSubState<Chattable> movetodeliverypoint;

		public GameStateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.DropSubState drop;

		public GameStateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.State success;
	}
}
