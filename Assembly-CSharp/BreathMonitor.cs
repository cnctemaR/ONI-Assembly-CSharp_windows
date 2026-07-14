using System;
using Klei.AI;
using TUNING;
using UnityEngine;

public class BreathMonitor : GameStateMachine<BreathMonitor, BreathMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.satisfied.DefaultState(this.satisfied.full).Transition(this.lowbreath, new StateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(BreathMonitor.IsLowBreath), UpdateRate.SIM_200ms);
		this.satisfied.full.Transition(this.satisfied.notfull, new StateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(BreathMonitor.IsNotFullBreath), UpdateRate.SIM_200ms).Enter(new StateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.State.Callback(BreathMonitor.HideBreathBar));
		this.satisfied.notfull.Transition(this.satisfied.full, new StateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(BreathMonitor.IsFullBreath), UpdateRate.SIM_200ms).Enter(new StateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.State.Callback(BreathMonitor.ShowBreathBar));
		this.lowbreath.DefaultState(this.lowbreath.nowheretorecover).Transition(this.satisfied, new StateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(BreathMonitor.IsFullBreath), UpdateRate.SIM_200ms).ToggleExpression(Db.Get().Expressions.RecoverBreath, new Func<BreathMonitor.Instance, bool>(BreathMonitor.IsOutOfOxygen))
			.ToggleUrge(Db.Get().Urges.RecoverBreath)
			.ToggleThought(Db.Get().Thoughts.Suffocating, null)
			.ToggleTag(GameTags.HoldingBreath)
			.Enter(new StateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.State.Callback(BreathMonitor.ShowBreathBar))
			.Enter(new StateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.State.Callback(BreathMonitor.UpdateRecoverBreathCell))
			.Update(new Action<BreathMonitor.Instance, float>(BreathMonitor.UpdateRecoverBreathCell), UpdateRate.RENDER_1000ms, true);
		this.lowbreath.nowheretorecover.ParamTransition<int>(this.recoverBreathCell, this.lowbreath.recoveryatcell, new StateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.Parameter<int>.Callback(BreathMonitor.IsValidRecoverCell)).ParamTransition<GameObject>(this.recoverBreathStation, this.lowbreath.recoveratstation, GameStateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.IsNotNull);
		this.lowbreath.recoveryatcell.ParamTransition<int>(this.recoverBreathCell, this.lowbreath.nowheretorecover, new StateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.Parameter<int>.Callback(BreathMonitor.IsNotValidRecoverCell)).ToggleChore(new Func<BreathMonitor.Instance, Chore>(BreathMonitor.CreateRecoverBreathChore), this.lowbreath.nowheretorecover);
		this.lowbreath.recoveratstation.ParamTransition<GameObject>(this.recoverBreathStation, this.lowbreath.nowheretorecover, GameStateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.IsNull).ToggleChore(new Func<BreathMonitor.Instance, Chore>(BreathMonitor.CreateBreathingStationChore), this.lowbreath.nowheretorecover);
	}

	private static bool IsLowBreath(BreathMonitor.Instance smi)
	{
		WorldContainer myWorld = smi.master.gameObject.GetMyWorld();
		if (!(myWorld == null) && myWorld.AlertManager.IsRedAlert())
		{
			return smi.breath.value < DUPLICANTSTATS.STANDARD.Breath.SUFFOCATE_AMOUNT;
		}
		return smi.breath.value < DUPLICANTSTATS.STANDARD.Breath.RETREAT_AMOUNT;
	}

	private static Chore CreateRecoverBreathChore(BreathMonitor.Instance smi)
	{
		return new RecoverBreathChore(smi.master);
	}

	private static Chore CreateBreathingStationChore(BreathMonitor.Instance smi)
	{
		UnderwaterBreathingLocation underwaterBreathingLocation = smi.sm.recoverBreathStation.Get<UnderwaterBreathingLocation>(smi);
		if (underwaterBreathingLocation != null)
		{
			UnderwaterBreathingLocationWorkable component = underwaterBreathingLocation.GetComponent<UnderwaterBreathingLocationWorkable>();
			if (component != null)
			{
				if (smi.swimMonitor.CanSwim() && Grid.IsLiquid(underwaterBreathingLocation.breathableCell))
				{
					component.workAnims = BreathMonitor.swimmingWorkAnims;
					component.workingPstComplete = BreathMonitor.swimmingWorkingPstAnims;
					component.workingPstFailed = BreathMonitor.swimmingWorkingPstAnims;
				}
				else
				{
					component.workAnims = BreathMonitor.landWorkAnims;
					component.workingPstComplete = BreathMonitor.landWorkingPstCompleteAnims;
					component.workingPstFailed = BreathMonitor.landWorkingPstCompleteAnims;
				}
				return new WorkChore<UnderwaterBreathingLocationWorkable>(Db.Get().ChoreTypes.RecoverBreath, component, null, true, null, new Action<Chore>(BreathMonitor.ReserveBreathLocation), new Action<Chore>(BreathMonitor.UnReserveBreathLocation), true, null, true, true, null, false, true, false, PriorityScreen.PriorityClass.compulsory, 5, false, true);
			}
		}
		return null;
	}

	private static void ReserveBreathLocation(Chore chore)
	{
		UnderwaterBreathingLocation underwaterBreathingLocation;
		if (chore.gameObject.TryGetComponent<UnderwaterBreathingLocation>(out underwaterBreathingLocation))
		{
			underwaterBreathingLocation.ReserveLocation(chore.driver.gameObject, true);
		}
	}

	private static void UnReserveBreathLocation(Chore chore)
	{
		UnderwaterBreathingLocation underwaterBreathingLocation;
		if (chore.gameObject.TryGetComponent<UnderwaterBreathingLocation>(out underwaterBreathingLocation))
		{
			underwaterBreathingLocation.ReserveLocation(chore.lastDriver.gameObject, false);
		}
	}

	private static bool IsNotFullBreath(BreathMonitor.Instance smi)
	{
		return !BreathMonitor.IsFullBreath(smi);
	}

	private static bool IsFullBreath(BreathMonitor.Instance smi)
	{
		return smi.breath.value >= smi.breath.GetMax();
	}

	private static bool IsOutOfOxygen(BreathMonitor.Instance smi)
	{
		return smi.breather.IsOutOfOxygen;
	}

	private static void ShowBreathBar(BreathMonitor.Instance smi)
	{
		if (NameDisplayScreen.Instance != null)
		{
			NameDisplayScreen.Instance.SetBreathDisplay(smi.gameObject, new Func<float>(smi.GetBreath), true);
		}
	}

	private static void HideBreathBar(BreathMonitor.Instance smi)
	{
		if (NameDisplayScreen.Instance != null)
		{
			NameDisplayScreen.Instance.SetBreathDisplay(smi.gameObject, null, false);
		}
	}

	private static bool IsValidRecoverCell(BreathMonitor.Instance smi, int cell)
	{
		return cell != Grid.InvalidCell;
	}

	private static bool IsNotValidRecoverCell(BreathMonitor.Instance smi, int cell)
	{
		return !BreathMonitor.IsValidRecoverCell(smi, cell);
	}

	private static void UpdateRecoverBreathCell(BreathMonitor.Instance smi, float dt)
	{
		BreathMonitor.UpdateRecoverBreathCell(smi);
	}

	private static void UpdateRecoverBreathCell(BreathMonitor.Instance smi)
	{
		if (smi.canRecoverBreath)
		{
			smi.query.Reset();
			smi.navigator.RunQuery(smi.query);
			int num = smi.query.GetResultCell();
			if (!GasBreatherFromWorldProvider.GetBestBreathableCellAroundSpecificCell(num, GasBreatherFromWorldProvider.DEFAULT_BREATHABLE_OFFSETS, smi.breather).IsBreathable)
			{
				num = PathFinder.InvalidCell;
			}
			bool flag = false;
			UnderwaterBreathingLocation underwaterBreathingLocation = BreathMonitor.FindNearestReachableStation(smi.navigator);
			if (underwaterBreathingLocation != null)
			{
				int num2 = smi.navigator.GetNavigationCost(num);
				if (num2 != -1 && Grid.IsSubstantialLiquid(smi.navigator.cachedCell, 0.35f))
				{
					num2 += BreathMonitor.breathableStationPreferenceCost;
				}
				int navigationCost = smi.navigator.GetNavigationCost(underwaterBreathingLocation.breathableCell);
				if ((num2 == -1 && navigationCost != -1) || num2 > navigationCost)
				{
					flag = true;
					smi.sm.recoverBreathStation.Set(underwaterBreathingLocation, smi);
					smi.sm.recoverBreathCell.Set(Grid.InvalidCell, smi, false);
				}
			}
			if (!flag)
			{
				smi.sm.recoverBreathStation.Set(null, smi);
				smi.sm.recoverBreathCell.Set(num, smi, false);
			}
		}
	}

	public static UnderwaterBreathingLocation FindNearestReachableStation(Navigator navigator)
	{
		UnderwaterBreathingLocation underwaterBreathingLocation = null;
		int num = int.MaxValue;
		for (int i = 0; i < Components.UnderwaterBreathingLocations.Count; i++)
		{
			UnderwaterBreathingLocation underwaterBreathingLocation2 = Components.UnderwaterBreathingLocations[i];
			if (underwaterBreathingLocation2.GetAvailableBreathableMass() > 0f && underwaterBreathingLocation2.CanReserve(navigator.gameObject))
			{
				int navigationCost = navigator.GetNavigationCost(underwaterBreathingLocation2.breathableCell);
				if (navigationCost != -1 && navigationCost < num)
				{
					num = navigationCost;
					underwaterBreathingLocation = underwaterBreathingLocation2;
				}
			}
		}
		return underwaterBreathingLocation;
	}

	private static HashedString[] swimmingWorkAnims = new HashedString[] { "working_pre", "working_loop" };

	private static HashedString[] swimmingWorkingPstAnims = new HashedString[] { "working_pst" };

	private static HashedString[] landWorkAnims = new HashedString[] { "working_land_pre", "working_land_loop" };

	private static HashedString[] landWorkingPstCompleteAnims = new HashedString[] { "working_land_pst" };

	public BreathMonitor.SatisfiedState satisfied;

	public BreathMonitor.LowBreathState lowbreath;

	public StateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.IntParameter recoverBreathCell;

	public StateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.TargetParameter recoverBreathStation;

	private static int breathableStationPreferenceCost = 15;

	public class LowBreathState : GameStateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.State nowheretorecover;

		public GameStateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.State recoveryatcell;

		public GameStateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.State recoveratstation;
	}

	public class SatisfiedState : GameStateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.State full;

		public GameStateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.State notfull;
	}

	public new class Instance : GameStateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.breath = Db.Get().Amounts.Breath.Lookup(master.gameObject);
			this.query = new SafetyQuery(Game.Instance.safetyConditions.RecoverBreathChecker, base.GetComponent<KMonoBehaviour>(), int.MaxValue);
			this.navigator = base.GetComponent<Navigator>();
			this.breather = base.GetComponent<OxygenBreather>();
			this.swimMonitor = base.gameObject.GetSMI<SwimMonitor.Instance>();
		}

		public int GetRecoverCell()
		{
			return base.sm.recoverBreathCell.Get(base.smi);
		}

		public float GetBreath()
		{
			return this.breath.value / this.breath.GetMax();
		}

		public AmountInstance breath;

		public SafetyQuery query;

		public Navigator navigator;

		public OxygenBreather breather;

		public bool canRecoverBreath = true;

		public SwimMonitor.Instance swimMonitor;
	}
}
