using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class EatChore : Chore<EatChore.StatesInstance>
{
	public EatChore(IStateMachineTarget master)
		: base(Db.Get().ChoreTypes.Eat, master, master.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.emergency, 0, false, true, 0, null)
	{
		this.smi = new EatChore.StatesInstance(this);
		this.showAvailabilityInHoverText = false;
		base.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		base.AddPrecondition(EatChore.EdibleIsNotNull, null);
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		if (context.consumerState.consumer == null)
		{
			global::Debug.LogError("EATCHORE null context.consumer", null);
			return;
		}
		RationMonitor.Instance smi = context.consumerState.consumer.GetSMI<RationMonitor.Instance>();
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
		KCrashReporter.Assert(edible.FoodInfo.CaloriesPerUnit > 0f, edible.GetProperName() + " has invalid calories per unit. Will result in NaNs");
		AmountInstance amountInstance = Db.Get().Amounts.Calories.Lookup(this.gameObject);
		float num = (amountInstance.GetMax() - amountInstance.value) / edible.FoodInfo.CaloriesPerUnit;
		KCrashReporter.Assert(num > 0f, "EatChore is requesting an invalid amount of food");
		this.smi.sm.requestedfoodunits.Set(num, this.smi);
		this.smi.sm.eater.Set(context.consumerState.gameObject, this.smi);
		base.Begin(context);
	}

	public static readonly Chore.Precondition EdibleIsNotNull = new Chore.Precondition
	{
		id = "EdibleIsNotNull",
		description = DUPLICANTS.CHORES.PRECONDITIONS.EDIBLE_IS_NOT_NULL,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return null != context.consumerState.consumer.GetSMI<RationMonitor.Instance>().GetEdible();
		}
	};

	public class StatesInstance : GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.GameInstance
	{
		public StatesInstance(EatChore master)
			: base(master)
		{
		}

		public void UpdateMessStation()
		{
			Ownables component = base.sm.eater.Get(base.smi).GetComponent<Ownables>();
			List<Assignable> preferredAssignables = Game.Instance.assignmentManager.GetPreferredAssignables(component, Db.Get().AssignableSlots.MessStation);
			Assignable assignable = ((preferredAssignables.Count <= 0) ? null : preferredAssignables[0]);
			if (assignable == null)
			{
				component.AutoAssignSlot(Db.Get().AssignableSlots.MessStation);
			}
			base.smi.sm.messstation.Set(assignable, base.smi);
		}

		public void CreateLocator()
		{
			int num = base.sm.eater.Get<Sensors>(base.smi).GetSensor<SafeCellSensor>().GetCell();
			if (num == Grid.InvalidCell)
			{
				num = Grid.PosToCell(base.sm.eater.Get<Transform>(base.smi).GetPosition());
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

		public void SetZ(GameObject go, float z)
		{
			Vector3 position = go.transform.GetPosition();
			position.z = z;
			go.transform.SetPosition(position);
		}

		public void ApplyRoomEffects()
		{
			Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.sm.messstation.Get(base.smi).gameObject);
			if (roomOfGameObject != null)
			{
				RoomType roomType = roomOfGameObject.roomType;
				roomType.TriggerRoomEffects(base.sm.messstation.Get(base.smi).gameObject.GetComponent<KPrefabID>(), base.sm.eater.Get(base.smi).gameObject.GetComponent<Effects>());
			}
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
			});
			this.fetch.InitializeStates(this.eater, this.ediblesource, this.ediblechunk, this.requestedfoodunits, this.actualfoodunits, this.eatatmessstation, null);
			this.eatatmessstation.DefaultState(this.eatatmessstation.moveto).ParamTransition<GameObject>(this.messstation, this.eatonfloorstate, (EatChore.StatesInstance smi, GameObject p) => p == null);
			this.eatatmessstation.moveto.InitializeStates(this.eater, this.messstation, this.eatatmessstation.eat, this.eatonfloorstate, null, null);
			this.eatatmessstation.eat.ToggleAnims("anim_eat_table_kanim", 0f).DoEat(this.ediblechunk, this.actualfoodunits, null, null).Enter(delegate(EatChore.StatesInstance smi)
			{
				smi.SetZ(this.eater.Get(smi), Grid.GetLayerZ(Grid.SceneLayer.BuildingFront));
				smi.ApplyRoomEffects();
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
			public GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.ApproachSubState<IApproachable> moveto;

			public GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State eat;
		}

		public class EatAtMessStationState : GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State
		{
			public GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.ApproachSubState<MessStation> moveto;

			public GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State eat;
		}
	}
}
