using System;
using Klei.AI;
using TUNING;

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
		this.lowbreath.nowheretorecover.ParamTransition<int>(this.recoverBreathCell, this.lowbreath.recoveryavailable, new StateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.Parameter<int>.Callback(BreathMonitor.IsValidRecoverCell));
		this.lowbreath.recoveryavailable.ParamTransition<int>(this.recoverBreathCell, this.lowbreath.nowheretorecover, new StateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.Parameter<int>.Callback(BreathMonitor.IsNotValidRecoverCell)).Enter(new StateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.State.Callback(BreathMonitor.UpdateRecoverBreathCell)).ToggleChore(new Func<BreathMonitor.Instance, Chore>(BreathMonitor.CreateRecoverBreathChore), this.lowbreath.nowheretorecover);
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
			smi.sm.recoverBreathCell.Set(num, smi, false);
		}
	}

	public BreathMonitor.SatisfiedState satisfied;

	public BreathMonitor.LowBreathState lowbreath;

	public StateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.IntParameter recoverBreathCell;

	public class LowBreathState : GameStateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.State nowheretorecover;

		public GameStateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.State recoveryavailable;
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
	}
}
