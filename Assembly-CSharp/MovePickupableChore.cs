using System;
using UnityEngine;

public class MovePickupableChore : Chore<MovePickupableChore.StatesInstance>
{
	public MovePickupableChore(IStateMachineTarget target, GameObject pickupable, Action<Chore> onEnd)
		: base(Db.Get().ChoreTypes.Fetch, target, target.GetComponent<ChoreProvider>(), false, null, null, onEnd, PriorityScreen.PriorityClass.basic, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new MovePickupableChore.StatesInstance(this);
		Pickupable component = pickupable.GetComponent<Pickupable>();
		base.AddPrecondition(ChorePreconditions.instance.CanPickup, component);
		base.AddPrecondition(ChorePreconditions.instance.CanMoveTo, target.GetComponent<Storage>());
		base.AddPrecondition(ChorePreconditions.instance.IsNotARobot, this);
		base.AddPrecondition(ChorePreconditions.instance.IsNotTransferArm, this);
		PrimaryElement component2 = pickupable.GetComponent<PrimaryElement>();
		base.smi.sm.requestedamount.Set(component2.Mass, base.smi, false);
		base.smi.sm.pickupablesource.Set(pickupable.gameObject, base.smi, false);
		base.smi.sm.deliverypoint.Set(target.gameObject, base.smi, false);
		this.movePlacer = target.gameObject;
		bool flag = MinionGroupProber.Get().IsReachable(Grid.PosToCell(pickupable), OffsetGroups.Standard) && MinionGroupProber.Get().IsReachable(Grid.PosToCell(target.gameObject), OffsetGroups.Standard);
		this.OnReachableChanged(flag);
		pickupable.Subscribe(-1432940121, new Action<object>(this.OnReachableChanged));
		target.Subscribe(-1432940121, new Action<object>(this.OnReachableChanged));
		Prioritizable component3 = target.GetComponent<Prioritizable>();
		if (!component3.IsPrioritizable())
		{
			component3.AddRef();
		}
		base.SetPrioritizable(target.GetComponent<Prioritizable>());
	}

	private void OnReachableChanged(object data)
	{
		Color color = (((bool)data) ? Color.white : new Color(0.91f, 0.21f, 0.2f));
		this.SetColor(this.movePlacer, color);
	}

	private void SetColor(GameObject visualizer, Color color)
	{
		if (visualizer != null)
		{
			visualizer.GetComponentInChildren<MeshRenderer>().material.color = color;
		}
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		if (context.consumerState.consumer == null)
		{
			global::Debug.LogError("MovePickupable null context.consumer");
			return;
		}
		if (base.smi == null)
		{
			global::Debug.LogError("MovePickupable null smi");
			return;
		}
		if (base.smi.sm == null)
		{
			global::Debug.LogError("MovePickupable null smi.sm");
			return;
		}
		if (base.smi.sm.pickupablesource == null)
		{
			global::Debug.LogError("MovePickupable null smi.sm.pickupablesource");
			return;
		}
		base.smi.sm.deliverer.Set(context.consumerState.gameObject, base.smi, false);
		base.Begin(context);
	}

	public GameObject movePlacer;

	public class StatesInstance : GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.GameInstance
	{
		public StatesInstance(MovePickupableChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.fetch;
			base.Target(this.deliverypoint);
			this.fetch.InitializeStates(this.deliverer, this.pickupablesource, this.pickup, this.requestedamount, this.actualamount, this.deliveryStorage, null);
			this.movetodeliverypoint.Target(this.deliverer).MoveTo((MovePickupableChore.StatesInstance smi) => Grid.PosToCell(this.deliverypoint.Get(smi)), this.drop, this.success, false);
			this.deliveryStorage.InitializeStates(this.deliverer, this.deliverypoint, this.delivering.storing, this.delivering.deliverfail, null, NavigationTactics.ReduceTravelDistance);
			this.delivering.storing.Target(this.deliverer).DoDelivery(this.deliverer, this.deliverypoint, this.success, this.delivering.deliverfail);
			this.delivering.deliverfail.ReturnFailure();
			this.success.Enter(delegate(MovePickupableChore.StatesInstance smi)
			{
				Storage component = this.deliverypoint.Get(smi).GetComponent<Storage>();
				Storage component2 = this.deliverer.Get(smi).GetComponent<Storage>();
				float num = this.actualamount.Get(smi);
				GameObject gameObject = this.pickup.Get(smi);
				num += gameObject.GetComponent<PrimaryElement>().Mass;
				this.actualamount.Set(num, smi, false);
				component2.Transfer(this.pickup.Get(smi), component, false, false);
				component.DropAll(false, false, default(Vector3), true, null);
				CancellableMove component3 = component.GetComponent<CancellableMove>();
				Movable component4 = gameObject.GetComponent<Movable>();
				component3.RemoveMovable(component4);
				component4.ClearMove();
				if (this.IsDeliveryComplete(smi))
				{
					smi.GoTo(this.success);
					return;
				}
				GameObject gameObject2 = this.pickupablesource.Get(smi);
				int num2 = Grid.PosToCell(this.deliverypoint.Get(smi));
				if (this.pickupablesource.Get(smi) == null || Grid.PosToCell(gameObject2) == num2)
				{
					this.pickupablesource.Set(component3.GetNextTarget(), smi, false);
				}
				smi.GoTo(this.fetch);
			}).ReturnSuccess();
		}

		private bool IsDeliveryComplete(MovePickupableChore.StatesInstance smi)
		{
			GameObject gameObject = smi.sm.deliverypoint.Get(smi);
			return !(gameObject != null) || gameObject.GetComponent<CancellableMove>().IsDeliveryComplete();
		}

		public StateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.TargetParameter deliverer;

		public StateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.TargetParameter pickupablesource;

		public StateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.TargetParameter pickup;

		public StateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.TargetParameter deliverypoint;

		public StateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.FloatParameter requestedamount;

		public StateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.FloatParameter actualamount;

		public GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.FetchSubState fetch;

		public GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.State movetodeliverypoint;

		public GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.ApproachSubState<Storage> deliveryStorage;

		public GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.DropSubState drop;

		public GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.State success;

		public MovePickupableChore.States.DeliveryState delivering;

		public class DeliveryState : GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.State
		{
			public GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.State storing;

			public GameStateMachine<MovePickupableChore.States, MovePickupableChore.StatesInstance, MovePickupableChore, object>.State deliverfail;
		}
	}
}
