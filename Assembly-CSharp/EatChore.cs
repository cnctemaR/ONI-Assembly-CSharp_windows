using System;
using Klei.AI;
using UnityEngine;

public class EatChore : Chore<EatChore.StatesInstance>
{
	public EatChore(IStateMachineTarget master)
		: base(Db.Get().ChoreTypes.Eat, master, master.GetComponent<ChoreProvider>(), false, null, null, null, int.MaxValue, false, true, 0)
	{
		this.smi = new EatChore.StatesInstance(this);
		base.AddPrecondition(ChorePreconditions.IsNotRedAlert, null);
		base.AddPrecondition(ChorePreconditions.IsScheduledTime, Db.Get().ScheduleBlockTypes.Eat);
		base.AddPrecondition(EatChore.EdibleIsNotNull, null);
	}

	// Note: this type is marked as 'beforefieldinit'.
	static EatChore()
	{
		Chore.Precondition precondition = default(Chore.Precondition);
		precondition.id = "EdibleIsNotNull";
		precondition.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return null != context.consumer.GetSMI<RationMonitor.Instance>().GetEdible();
		};
		EatChore.EdibleIsNotNull = precondition;
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		if (context.consumer == null)
		{
			global::Debug.LogError("EATCHORE null context.consumer", null);
			return;
		}
		RationMonitor.Instance smi = context.consumer.GetSMI<RationMonitor.Instance>();
		if (smi == null)
		{
			global::Debug.LogError("EATCHORE null RationMonitor.Instance", null);
			return;
		}
		Edible edible = smi.GetEdible();
		if (edible.gameObject == null)
		{
			global::Debug.LogError("EATCHORE null edible.gameObject", null);
			return;
		}
		if (this.smi == null)
		{
			global::Debug.LogError("EATCHORE null smi", null);
			return;
		}
		if (this.smi.sm == null)
		{
			global::Debug.LogError("EATCHORE null smi.sm", null);
			return;
		}
		if (this.smi.sm.ediblesource == null)
		{
			global::Debug.LogError("EATCHORE null smi.sm.ediblesource", null);
			return;
		}
		this.smi.sm.ediblesource.Set(edible.gameObject, this.smi);
		AmountInstance amountInstance = Db.Get().Amounts.Calories.Lookup(this.gameObject);
		float num = (amountInstance.GetMax() - amountInstance.value) / edible.FoodInfo.CaloriesPerUnit;
		this.smi.sm.requestedfoodunits.Set(num, this.smi);
		this.smi.sm.eater.Set(context.consumer.gameObject, this.smi);
		base.Begin(context);
	}

	public static Chore.Precondition EdibleIsNotNull;

	public class StatesInstance : GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.GameInstance
	{
		public StatesInstance(EatChore master)
			: base(master)
		{
		}

		public void UpdateMessStation()
		{
			Ownables component = base.sm.eater.Get(base.smi).GetComponent<Ownables>();
			Navigator component2 = component.GetComponent<Navigator>();
			Assignable assignable = component.AutoAssignSlot(component2, Db.Get().OwnableSlots.MessStation);
			base.smi.sm.messstation.Set(assignable, base.smi);
		}

		public void CreateLocator()
		{
			int num = base.sm.eater.Get<Sensors>(base.smi).GetSensor<SafeCellSensor>().GetCell();
			if (num == Grid.InvalidCell)
			{
				num = Grid.PosToCell(base.sm.eater.Get<Transform>(base.smi).position);
			}
			Vector3 vector = Grid.CellToPosCBC(num, Grid.SceneLayer.Move);
			Grid.Reserved[num] = true;
			GameObject gameObject = ChoreHelpers.CreateLocator("EatLocator", vector);
			base.sm.locator.Set(gameObject, this);
			this.locatorCell = num;
		}

		public void DestroyLocator()
		{
			Grid.Reserved[this.locatorCell] = false;
			ChoreHelpers.DestroyLocator(base.sm.locator.Get(this));
			base.sm.locator.Set(null, this);
		}

		public bool IsInterrupted()
		{
			return true && !base.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Eat);
		}

		public void SetZ(GameObject go, float z)
		{
			Vector3 position = go.transform.position;
			position.z = z;
			go.transform.position = position;
		}

		private int locatorCell;
	}

	public class States : GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.fetch;
			base.Target(this.eater);
			this.root.Enter("SetMessStation", delegate(EatChore.StatesInstance smi)
			{
				smi.UpdateMessStation();
			}).EventHandler(GameHashes.AssignablesChanged, delegate(EatChore.StatesInstance smi)
			{
				smi.UpdateMessStation();
			}).EventTransition(GameHashes.ScheduleChanged, this.interruptedbyschedule, (EatChore.StatesInstance smi) => smi.IsInterrupted());
			this.fetch.InitializeStates(this.eater, this.ediblesource, this.ediblechunk, this.requestedfoodunits, this.actualfoodunits, this.eatatmessstation, null);
			this.eatatmessstation.DefaultState(this.eatatmessstation.moveto).ParamTransition<GameObject>(this.messstation, this.eatonfloorstate, (EatChore.StatesInstance smi, GameObject p) => p == null);
			this.eatatmessstation.moveto.InitializeStates(this.eater, this.messstation, this.eatatmessstation.eat, this.eatonfloorstate, null, null);
			this.eatatmessstation.eat.ToggleAnims("anim_eat_table_kanim", 0f).DoEat(this.ediblechunk, this.actualfoodunits, null, null).Enter(delegate(EatChore.StatesInstance smi)
			{
				smi.SetZ(this.eater.Get(smi), Grid.GetLayerZ(Grid.SceneLayer.BuildingFront));
			})
				.Exit(delegate(EatChore.StatesInstance smi)
				{
					smi.SetZ(this.eater.Get(smi), Grid.GetLayerZ(Grid.SceneLayer.Move));
				});
			this.eatonfloorstate.DefaultState(this.eatonfloorstate.moveto).Enter("CreateLocator", delegate(EatChore.StatesInstance smi)
			{
				smi.CreateLocator();
			}).Exit("DestroyLocator", delegate(EatChore.StatesInstance smi)
			{
				smi.DestroyLocator();
			});
			this.eatonfloorstate.moveto.InitializeStates(this.eater, this.locator, this.eatonfloorstate.eat, this.eatonfloorstate.eat, null, null);
			this.eatonfloorstate.eat.ToggleAnims("anim_eat_floor_kanim", 0f).DoEat(this.ediblechunk, this.actualfoodunits, null, null);
			this.interruptedbyschedule.GoTo(null);
		}

		public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.TargetParameter eater;

		public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.TargetParameter ediblesource;

		public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.TargetParameter ediblechunk;

		public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.TargetParameter messstation;

		public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.FloatParameter requestedfoodunits;

		public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.FloatParameter actualfoodunits;

		public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.TargetParameter locator;

		public GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State interruptedbyschedule;

		public GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.FetchSubState fetch;

		public EatChore.States.EatOnFloorState eatonfloorstate;

		public EatChore.States.EatAtMessStationState eatatmessstation;

		public class EatOnFloorState : GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State
		{
			public GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.ApproachSubState<Approachable> moveto;

			public GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State eat;
		}

		public class EatAtMessStationState : GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State
		{
			public GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.ApproachSubState<MessStation> moveto;

			public GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State eat;
		}
	}
}
