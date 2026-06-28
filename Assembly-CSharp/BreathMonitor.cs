using System;
using Klei.AI;

public class BreathMonitor : GameStateMachine<BreathMonitor, BreathMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.satisfied.Transition(this.lowbreath, (BreathMonitor.Instance smi) => smi.IsLowBreath());
		this.lowbreath.DefaultState(this.lowbreath.nowheretorecover).ToggleExpression(Db.Get().Expressions.RecoverBreath, (BreathMonitor.Instance smi) => !smi.IsInBreathableArea()).ToggleUrge(Db.Get().Urges.RecoverBreath)
			.Update("UpdateRecoverBreathCell", delegate(BreathMonitor.Instance smi)
			{
				smi.UpdateRecoverBreathCell();
			})
			.ToggleThought(Db.Get().Thoughts.Suffocating, null)
			.Transition(this.satisfied, (BreathMonitor.Instance smi) => smi.HasRecoveredBreath());
		this.lowbreath.nowheretorecover.ParamTransition<int>(this.recoverBreathCell, this.lowbreath.recoveryavailable, (BreathMonitor.Instance smi, int p) => p != Grid.InvalidCell);
		this.lowbreath.recoveryavailable.ParamTransition<int>(this.recoverBreathCell, this.lowbreath.nowheretorecover, (BreathMonitor.Instance smi, int p) => p == Grid.InvalidCell).ToggleChore((BreathMonitor.Instance smi) => new RecoverBreathChore(smi.master), this.lowbreath.nowheretorecover, false);
	}

	public GameStateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.State satisfied;

	public BreathMonitor.LowBreathState lowbreath;

	public StateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.IntParameter recoverBreathCell;

	public class LowBreathState : GameStateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.State nowheretorecover;

		public GameStateMachine<BreathMonitor, BreathMonitor.Instance, IStateMachineTarget, object>.State recoveryavailable;
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
			return this.breather.IsBreathableElementAtCell(Grid.PosToCell(base.transform.position), null);
		}

		public bool HasRecoveredBreath()
		{
			return this.breath.value >= this.breath.GetMax() - 1f;
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

		private AmountInstance breath;

		private SafetyQuery query;

		private Navigator navigator;

		private OxygenBreather breather;
	}
}
