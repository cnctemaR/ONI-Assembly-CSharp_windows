using System;
using Klei.AI;

public class BreathMonitor : GameStateMachine<BreathMonitor, BreathMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.satisfied.DefaultState(this.satisfied.full).Transition(this.lowbreath, (BreathMonitor.Instance smi) => smi.IsLowBreath(), UpdateRate.SIM_200ms);
		this.satisfied.full.Transition(this.satisfied.notfull, (BreathMonitor.Instance smi) => !smi.IsFullBreath(), UpdateRate.SIM_200ms).Enter("BreathBar", delegate(BreathMonitor.Instance smi)
		{
			if (NameDisplayScreen.Instance != null)
			{
				NameDisplayScreen.Instance.SetBreathDisplay(smi.master.gameObject, new Func<float>(smi.GetBreath), false);
			}
		});
		this.satisfied.notfull.Transition(this.satisfied.full, (BreathMonitor.Instance smi) => smi.IsFullBreath(), UpdateRate.SIM_200ms).Enter("BreathBar", delegate(BreathMonitor.Instance smi)
		{
			if (NameDisplayScreen.Instance != null)
			{
				NameDisplayScreen.Instance.SetBreathDisplay(smi.master.gameObject, new Func<float>(smi.GetBreath), true);
			}
		});
		this.lowbreath.DefaultState(this.lowbreath.nowheretorecover).Transition(this.satisfied, (BreathMonitor.Instance smi) => smi.IsFullBreath(), UpdateRate.SIM_200ms).ToggleExpression(Db.Get().Expressions.RecoverBreath, (BreathMonitor.Instance smi) => !smi.IsInBreathableArea())
			.ToggleUrge(Db.Get().Urges.RecoverBreath)
			.ToggleThought(Db.Get().Thoughts.Suffocating, null)
			.Enter("BreathBar", delegate(BreathMonitor.Instance smi)
			{
				if (NameDisplayScreen.Instance != null)
				{
					NameDisplayScreen.Instance.SetBreathDisplay(smi.master.gameObject, new Func<float>(smi.GetBreath), true);
				}
			})
			.Update("UpdateRecoverBreathCell", delegate(BreathMonitor.Instance smi, float dt)
			{
				smi.UpdateRecoverBreathCell();
			}, UpdateRate.SIM_200ms, false);
		this.lowbreath.nowheretorecover.ParamTransition<int>(this.recoverBreathCell, this.lowbreath.recoveryavailable, (BreathMonitor.Instance smi, int p) => p != Grid.InvalidCell);
		this.lowbreath.recoveryavailable.ParamTransition<int>(this.recoverBreathCell, this.lowbreath.nowheretorecover, (BreathMonitor.Instance smi, int p) => p == Grid.InvalidCell).ToggleChore((BreathMonitor.Instance smi) => new RecoverBreathChore(smi.master), this.lowbreath.nowheretorecover);
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

		public bool IsInBreathableArea()
		{
			return this.breather.IsBreathableElementAtCell(Grid.PosToCell(base.transform.GetPosition()), null);
		}

		public bool IsFullBreath()
		{
			return this.breath.value >= this.breath.GetMax();
		}

		public bool IsLowBreath()
		{
			return this.breath.value < 72.72727f;
		}

		public int GetRecoverCell()
		{
			return base.sm.recoverBreathCell.Get(base.smi);
		}

		public void UpdateRecoverBreathCell()
		{
			this.query.Reset();
			this.navigator.RunQuery(this.query);
			int num = this.query.GetResultCell();
			if (!this.breather.IsBreathableElementAtCell(num, null))
			{
				num = PathFinder.InvalidCell;
			}
			base.sm.recoverBreathCell.Set(num, base.smi);
		}

		public float GetBreath()
		{
			return this.breath.value / this.breath.GetMax();
		}

		private AmountInstance breath;

		private SafetyQuery query;

		private Navigator navigator;

		private OxygenBreather breather;
	}
}
