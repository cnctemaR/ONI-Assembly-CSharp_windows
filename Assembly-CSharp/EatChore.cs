using System;
using System.Collections.Generic;
using FoodRehydrator;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class EatChore : Chore<EatChore.StatesInstance>
{
	public static IDiningSeat ResolveDiningSeat(GameObject messStation)
	{
		if (messStation == null)
		{
			global::Debug.LogWarning("messStation GameObject is null");
			return null;
		}
		IDiningSeat diningSeat;
		if (!messStation.TryGetComponent<IDiningSeat>(out diningSeat))
		{
			global::Debug.LogWarning("messStation GameObject has no IDiningSeat component");
			return null;
		}
		return diningSeat;
	}

	private static KAnimFile ResolveEatAnim(IDiningSeat diningSeat, bool dinerIsBionic)
	{
		HashedString hashedString = ((diningSeat != null) ? (dinerIsBionic ? diningSeat.ReloadElectrobankAnim : diningSeat.EatAnim) : MessStation.eatAnim);
		KAnimFile anim = Assets.GetAnim(hashedString);
		if (anim == null)
		{
			global::Debug.LogError(string.Format("Animation asset [{0}] does not exist", hashedString));
			return null;
		}
		return anim;
	}

	private static KAnimFile ResolveEatAnim(GameObject messStation, bool dinerIsBionic)
	{
		return EatChore.ResolveEatAnim(EatChore.ResolveDiningSeat(messStation), dinerIsBionic);
	}

	public EatChore(IStateMachineTarget master)
		: base(Db.Get().ChoreTypes.Eat, master, master.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.personalNeeds, 5, false, true, 0, false, ReportManager.ReportType.PersonalTime)
	{
		base.smi = new EatChore.StatesInstance(this);
		this.showAvailabilityInHoverText = false;
		this.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		this.AddPrecondition(EatChore.EdibleIsNotNull, null);
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

	public static bool IsMessStationNonOperational(GameObject messStation)
	{
		if (messStation == null)
		{
			return true;
		}
		IDiningSeat diningSeat = EatChore.ResolveDiningSeat(messStation);
		if (diningSeat == null)
		{
			return true;
		}
		Operational operational = diningSeat.FindOperational();
		return operational == null || !operational.IsOperational;
	}

	private static bool IsMessStationNonOperational(EatChore.StatesInstance _, GameObject messStation)
	{
		return EatChore.IsMessStationNonOperational(messStation);
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

		private static Assignable GetPreferredMessStation(GameObject diner)
		{
			Ownables soleOwner = diner.GetComponent<MinionIdentity>().GetSoleOwner();
			Navigator navigator;
			diner.TryGetComponent<Navigator>(out navigator);
			foreach (Assignable assignable in Game.Instance.assignmentManager.GetPreferredAssignables(soleOwner, navigator, Db.Get().AssignableSlots.MessStation))
			{
				if (EatChore.ResolveDiningSeat(assignable.gameObject) != null && assignable.GetComponent<Reservable>().IsReservableBy(diner))
				{
					return assignable;
				}
			}
			return null;
		}

		public static Assignable ReserveMessStation(GameObject messStation, GameObject diner)
		{
			if (messStation != null)
			{
				messStation.GetComponent<Reservable>().ClearReservation();
			}
			Assignable preferredMessStation = EatChore.StatesInstance.GetPreferredMessStation(diner);
			if (preferredMessStation != null && !preferredMessStation.GetComponent<Reservable>().Reserve(diner))
			{
				global::Debug.Log("Failed to reserve dining seat. We have likely already reserved it.");
			}
			return preferredMessStation;
		}

		public void UpdateMessStation()
		{
			Assignable assignable = EatChore.StatesInstance.ReserveMessStation(base.sm.messstation.Get(base.smi), base.sm.eater.Get(base.smi));
			base.sm.messstation.Set(assignable, base.smi);
		}

		public void ClearMessStation()
		{
			GameObject gameObject = base.smi.sm.messstation.Get(base.smi);
			if (gameObject != null)
			{
				gameObject.GetComponent<Reservable>().ClearReservation();
			}
			base.sm.messstation.Set(null, base.smi);
		}

		public static bool UseSalt(GameObject messStation)
		{
			if (messStation == null)
			{
				return false;
			}
			IDiningSeat diningSeat = EatChore.ResolveDiningSeat(messStation);
			return diningSeat != null && diningSeat.HasSalt;
		}

		public bool UseSalt()
		{
			return base.smi.sm.messstation != null && EatChore.StatesInstance.UseSalt(base.sm.messstation.Get(base.smi));
		}

		public static ValueTuple<GameObject, int> CreateLocator(Sensors sensors, Transform transform, string locatorName)
		{
			int num = sensors.GetSensor<SafeCellSensor>().GetCellQuery();
			if (num == Grid.InvalidCell)
			{
				num = Grid.PosToCell(transform.GetPosition());
			}
			Vector3 vector = Grid.CellToPosCBC(num, Grid.SceneLayer.Move);
			Grid.Reserved[num] = true;
			return new ValueTuple<GameObject, int>(ChoreHelpers.CreateLocator(locatorName, vector), num);
		}

		public void CreateLocator()
		{
			ValueTuple<GameObject, int> valueTuple = EatChore.StatesInstance.CreateLocator(base.sm.eater.Get<Sensors>(base.smi), base.sm.eater.Get<Transform>(base.smi), "EatLocator");
			GameObject item = valueTuple.Item1;
			this.locatorCell = valueTuple.Item2;
			base.sm.locator.Set(item, this, false);
		}

		public void DestroyLocator()
		{
			Grid.Reserved[this.locatorCell] = false;
			ChoreHelpers.DestroyLocator(base.sm.locator.Get(this));
			base.sm.locator.Set(null, this);
		}

		public static KAnimFile OnEnterMessStation(GameObject messStation, GameObject diner, GameObject food, bool dinerIsBionic, float? effectDurationOverride = null)
		{
			IDiningSeat diningSeat = EatChore.ResolveDiningSeat(messStation);
			if (diningSeat == null)
			{
				return null;
			}
			KAnimControllerBase component = diner.GetComponent<KAnimControllerBase>();
			KAnimFile kanimFile = EatChore.ResolveEatAnim(diningSeat, dinerIsBionic);
			component.AddAnimOverrides(kanimFile, 0f);
			Edible edible;
			if (food != null && food.TryGetComponent<Edible>(out edible))
			{
				edible.workLayer = Grid.SceneLayer.BuildingFront;
			}
			EffectInstance effectInstance = null;
			Effects component2 = diner.GetComponent<Effects>();
			Storage storage = diningSeat.FindStorage();
			if (storage != null && storage.Has(TableSaltConfig.TAG))
			{
				storage.ConsumeIgnoringDisease(TableSaltConfig.TAG, TableSaltTuning.CONSUMABLE_RATE);
				effectInstance = component2.Add("MessTableSalt", true);
			}
			diningSeat.Diner = diner.GetComponent<KPrefabID>();
			messStation.Trigger(1356255274, null);
			Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(messStation);
			KPrefabID component3 = messStation.GetComponent<KPrefabID>();
			if (effectDurationOverride != null)
			{
				List<EffectInstance> list = null;
				if (roomOfGameObject != null)
				{
					roomOfGameObject.roomType.TriggerRoomEffects(component3, component2, out list);
				}
				if (effectInstance != null)
				{
					if (list == null)
					{
						list = new List<EffectInstance>();
					}
					list.Add(effectInstance);
				}
				if (list == null)
				{
					return kanimFile;
				}
				using (List<EffectInstance>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						EffectInstance effectInstance2 = enumerator.Current;
						effectInstance2.timeRemaining = effectDurationOverride.Value;
					}
					return kanimFile;
				}
			}
			if (roomOfGameObject != null)
			{
				roomOfGameObject.roomType.TriggerRoomEffects(component3, component2);
			}
			return kanimFile;
		}

		public static void OnExitMessStation(GameObject messStation, GameObject diner, KAnimFile eatAnim)
		{
			diner.GetComponent<KAnimControllerBase>().RemoveAnimOverrides(eatAnim);
			IDiningSeat diningSeat = EatChore.ResolveDiningSeat(messStation);
			if (diningSeat != null)
			{
				diningSeat.Diner = null;
			}
		}

		private int locatorCell;

		public KAnimFile eatAnim;
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
			}).Exit(delegate(EatChore.StatesInstance smi)
			{
				smi.ClearMessStation();
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
				WorkerBase workerBase = this.eater.Get<WorkerBase>(smi);
				DehydratedFoodPackage dehydratedFoodPackage = this.rehydrate.foodpackage.Get<DehydratedFoodPackage>(smi);
				workerBase.StartWork(new DehydratedFoodPackage.RehydrateStartWorkItem(dehydratedFoodPackage, delegate(GameObject result)
				{
					this.ediblechunk.Set(result, smi, false);
				}));
			}, delegate(EatChore.StatesInstance smi)
			{
				AccessabilityManager accessabilityManager3 = this.rehydrate.rehydrator.Get(smi);
				return !(accessabilityManager3 == null) && accessabilityManager3.CanAccess(this.eater.Get<WorkerBase>(smi).gameObject);
			}, this.eatatmessstation, null);
			this.fetch.InitializeStates(this.eater, this.ediblesource, this.ediblechunk, this.requestedfoodunits, this.actualfoodunits, this.eatatmessstation, null);
			this.eatatmessstation.DefaultState(this.eatatmessstation.moveto).ParamTransition<GameObject>(this.messstation, this.eatonfloorstate, (EatChore.StatesInstance smi, GameObject p) => p == null).ParamTransition<GameObject>(this.messstation, this.eatonfloorstate, new StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.Parameter<GameObject>.Callback(EatChore.IsMessStationNonOperational));
			this.eatatmessstation.moveto.InitializeStates(this.eater, this.messstation, this.eatatmessstation.eat, this.eatonfloorstate, null, null);
			this.eatatmessstation.eat.Enter("OnEnterMessStation", delegate(EatChore.StatesInstance smi)
			{
				smi.eatAnim = EatChore.StatesInstance.OnEnterMessStation(this.messstation.Get(smi), this.eater.Get(smi), this.ediblechunk.Get(smi), false, null);
			}).DoEat(this.ediblechunk, this.actualfoodunits, null, null).Exit(delegate(EatChore.StatesInstance smi)
			{
				EatChore.StatesInstance.OnExitMessStation(this.messstation.Get(smi), this.eater.Get(smi), smi.eatAnim);
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
			public GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.ApproachSubState<IApproachable> moveto;

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
