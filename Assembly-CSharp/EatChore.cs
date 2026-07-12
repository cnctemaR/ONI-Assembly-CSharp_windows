using System;
using System.Collections.Generic;
using FoodRehydrator;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class EatChore : Chore<EatChore.StatesInstance>
{
	public EatChore(IStateMachineTarget master)
		: base(Db.Get().ChoreTypes.Eat, master, master.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.personalNeeds, 5, false, true, 0, false, ReportManager.ReportType.PersonalTime)
	{
		base.smi = new EatChore.StatesInstance(this);
		this.showAvailabilityInHoverText = false;
		base.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		base.AddPrecondition(EatChore.EdibleIsNotNull, null);
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		if (context.consumerState.consumer == null)
		{
			global::Debug.LogError("EATCHORE null context.consumer");
			return;
		}
		RationMonitor.Instance smi = context.consumerState.consumer.GetSMI<RationMonitor.Instance>();
		if (smi == null)
		{
			global::Debug.LogError("EATCHORE null RationMonitor.Instance");
			return;
		}
		Edible edible = smi.GetEdible();
		if (edible.gameObject == null)
		{
			global::Debug.LogError("EATCHORE null edible.gameObject");
			return;
		}
		if (base.smi == null)
		{
			global::Debug.LogError("EATCHORE null smi");
			return;
		}
		if (base.smi.sm == null)
		{
			global::Debug.LogError("EATCHORE null smi.sm");
			return;
		}
		if (base.smi.sm.ediblesource == null)
		{
			global::Debug.LogError("EATCHORE null smi.sm.ediblesource");
			return;
		}
		base.smi.sm.ediblesource.Set(edible.gameObject, base.smi, false);
		KCrashReporter.Assert(edible.FoodInfo.CaloriesPerUnit > 0f, edible.GetProperName() + " has invalid calories per unit. Will result in NaNs", null);
		AmountInstance amountInstance = Db.Get().Amounts.Calories.Lookup(this.gameObject);
		float num = (amountInstance.GetMax() - amountInstance.value) / edible.FoodInfo.CaloriesPerUnit;
		KCrashReporter.Assert(num > 0f, "EatChore is requesting an invalid amount of food", null);
		base.smi.sm.requestedfoodunits.Set(num, base.smi, false);
		base.smi.sm.eater.Set(context.consumerState.gameObject, base.smi, false);
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
			Ownables soleOwner = base.sm.eater.Get(base.smi).GetComponent<MinionIdentity>().GetSoleOwner();
			List<Assignable> list = Game.Instance.assignmentManager.GetPreferredAssignables(soleOwner, Db.Get().AssignableSlots.MessStation);
			if (list.Count == 0)
			{
				soleOwner.AutoAssignSlot(Db.Get().AssignableSlots.MessStation);
				list = Game.Instance.assignmentManager.GetPreferredAssignables(soleOwner, Db.Get().AssignableSlots.MessStation);
			}
			Assignable assignable = ((list.Count > 0) ? list[0] : null);
			base.smi.sm.messstation.Set(assignable, base.smi);
		}

		public bool UseSalt()
		{
			if (base.smi.sm.messstation != null && base.smi.sm.messstation.Get(base.smi) != null)
			{
				MessStation component = base.smi.sm.messstation.Get(base.smi).GetComponent<MessStation>();
				return component != null && component.HasSalt;
			}
			return false;
		}

		public void CreateLocator()
		{
			int num = base.sm.eater.Get<Sensors>(base.smi).GetSensor<SafeCellSensor>().GetCellQuery();
			if (num == Grid.InvalidCell)
			{
				num = Grid.PosToCell(base.sm.eater.Get<Transform>(base.smi).GetPosition());
			}
			Vector3 vector = Grid.CellToPosCBC(num, Grid.SceneLayer.Move);
			Grid.Reserved[num] = true;
			GameObject gameObject = ChoreHelpers.CreateLocator("EatLocator", vector);
			base.sm.locator.Set(gameObject, this, false);
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
				roomOfGameObject.roomType.TriggerRoomEffects(base.sm.messstation.Get(base.smi).gameObject.GetComponent<KPrefabID>(), base.sm.eater.Get(base.smi).gameObject.GetComponent<Effects>());
			}
		}

		public void ApplySaltEffect()
		{
			Storage component = base.sm.messstation.Get(base.smi).gameObject.GetComponent<Storage>();
			if (component != null && component.Has(TableSaltConfig.ID.ToTag()))
			{
				component.ConsumeIgnoringDisease(TableSaltConfig.ID.ToTag(), TableSaltTuning.CONSUMABLE_RATE);
				base.sm.eater.Get(base.smi).gameObject.GetComponent<Worker>().GetComponent<Effects>().Add("MessTableSalt", true);
				base.sm.messstation.Get(base.smi).gameObject.Trigger(1356255274, null);
			}
		}

		private int locatorCell;
	}

	public class States : GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.chooseaction;
			base.Target(this.eater);
			this.root.Enter("SetMessStation", delegate(EatChore.StatesInstance smi)
			{
				smi.UpdateMessStation();
			}).EventHandler(GameHashes.AssignablesChanged, delegate(EatChore.StatesInstance smi)
			{
				smi.UpdateMessStation();
			});
			this.chooseaction.EnterTransition(this.rehydrate, (EatChore.StatesInstance smi) => this.ediblesource.Get(smi).HasTag(GameTags.Dehydrated)).EnterTransition(this.fetch, (EatChore.StatesInstance smi) => true);
			this.rehydrate.Enter(delegate(EatChore.StatesInstance smi)
			{
				DehydratedFoodPackage component = this.ediblesource.Get(smi).GetComponent<Pickupable>().storage.gameObject.GetComponent<DehydratedFoodPackage>();
				this.rehydrate.foodpackage.Set(component, smi);
				GameObject rehydrator = component.Rehydrator;
				this.rehydrate.rehydrator.Set((rehydrator != null) ? component.Rehydrator.GetComponent<AccessabilityManager>() : null, smi, false);
				AccessabilityManager accessabilityManager = this.rehydrate.rehydrator.Get(smi);
				if (!(accessabilityManager != null))
				{
					smi.GoTo(null);
					return;
				}
				GameObject gameObject = this.eater.Get(smi);
				if (accessabilityManager.CanAccess(gameObject))
				{
					accessabilityManager.Reserve(this.eater.Get(smi));
					return;
				}
				smi.GoTo(null);
			}).Exit(delegate(EatChore.StatesInstance smi)
			{
				AccessabilityManager accessabilityManager2 = this.rehydrate.rehydrator.Get(smi);
				if (accessabilityManager2 != null)
				{
					accessabilityManager2.Unreserve();
				}
			}).DefaultState(this.rehydrate.approach);
			this.rehydrate.approach.InitializeStates(this.eater, this.rehydrate.foodpackage, this.rehydrate.work, null, null, NavigationTactics.ReduceTravelDistance).OnTargetLost(this.ediblesource, null);
			this.rehydrate.work.ToggleWork("Rehydrate", delegate(EatChore.StatesInstance smi)
			{
				Worker worker = this.eater.Get<Worker>(smi);
				DehydratedFoodPackage dehydratedFoodPackage = this.rehydrate.foodpackage.Get<DehydratedFoodPackage>(smi);
				worker.StartWork(new DehydratedFoodPackage.RehydrateStartWorkItem(dehydratedFoodPackage, delegate(GameObject result)
				{
					this.ediblechunk.Set(result, smi, false);
				}));
			}, delegate(EatChore.StatesInstance smi)
			{
				AccessabilityManager accessabilityManager3 = this.rehydrate.rehydrator.Get(smi);
				return !(accessabilityManager3 == null) && accessabilityManager3.CanAccess(this.eater.Get<Worker>(smi).gameObject);
			}, this.eatatmessstation, null);
			this.fetch.InitializeStates(this.eater, this.ediblesource, this.ediblechunk, this.requestedfoodunits, this.actualfoodunits, this.eatatmessstation, null);
			this.eatatmessstation.DefaultState(this.eatatmessstation.moveto).ParamTransition<GameObject>(this.messstation, this.eatonfloorstate, (EatChore.StatesInstance smi, GameObject p) => p == null).ParamTransition<GameObject>(this.messstation, this.eatonfloorstate, (EatChore.StatesInstance smi, GameObject p) => p != null && !p.GetComponent<Operational>().IsOperational);
			this.eatatmessstation.moveto.InitializeStates(this.eater, this.messstation, this.eatatmessstation.eat, this.eatonfloorstate, null, null);
			this.eatatmessstation.eat.Enter("AnimOverride", delegate(EatChore.StatesInstance smi)
			{
				smi.GetComponent<KAnimControllerBase>().AddAnimOverrides(Assets.GetAnim("anim_eat_table_kanim"), 0f);
			}).DoEat(this.ediblechunk, this.actualfoodunits, null, null).Enter(delegate(EatChore.StatesInstance smi)
			{
				smi.SetZ(this.eater.Get(smi), Grid.GetLayerZ(Grid.SceneLayer.BuildingFront));
				smi.ApplyRoomEffects();
				smi.ApplySaltEffect();
			})
				.Exit(delegate(EatChore.StatesInstance smi)
				{
					smi.SetZ(this.eater.Get(smi), Grid.GetLayerZ(Grid.SceneLayer.Move));
					smi.GetComponent<KAnimControllerBase>().RemoveAnimOverrides(Assets.GetAnim("anim_eat_table_kanim"));
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
		}

		public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.TargetParameter eater;

		public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.TargetParameter ediblesource;

		public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.TargetParameter ediblechunk;

		public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.TargetParameter messstation;

		public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.FloatParameter requestedfoodunits;

		public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.FloatParameter actualfoodunits;

		public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.TargetParameter locator;

		public GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State chooseaction;

		public EatChore.States.RehydrateSubState rehydrate;

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

		public class RehydrateSubState : GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State
		{
			public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.TargetParameter foodpackage;

			public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.ObjectParameter<AccessabilityManager> rehydrator;

			public GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.ApproachSubState<DehydratedFoodPackage> approach;

			public GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State work;
		}
	}
}
