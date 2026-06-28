using System;
using Klei.AI;

public class TemperatureMonitor : GameStateMachine<TemperatureMonitor, TemperatureMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.satisfied.Transition(this.hot, (TemperatureMonitor.Instance smi) => smi.IsHot()).Transition(this.cold, (TemperatureMonitor.Instance smi) => smi.IsCold());
		this.hot.Transition(this.satisfied, (TemperatureMonitor.Instance smi) => !smi.IsHot()).Transition(this.deathhot, (TemperatureMonitor.Instance smi) => smi.IsOverHeated()).ToggleExpression(Db.Get().Expressions.Hot, null)
			.ToggleThought(Db.Get().Thoughts.Hot, null)
			.ToggleStatusItem(Db.Get().DuplicantStatusItems.Hot, null);
		this.deathhot.Enter("KillHot", delegate(TemperatureMonitor.Instance smi)
		{
			smi.KillHot();
		});
		this.cold.Transition(this.cold, (TemperatureMonitor.Instance smi) => !smi.IsCold()).Transition(this.deathcold, (TemperatureMonitor.Instance smi) => smi.IsFrozenSolid()).ToggleExpression(Db.Get().Expressions.Cold, null)
			.ToggleThought(Db.Get().Thoughts.Cold, null)
			.ToggleStatusItem(Db.Get().DuplicantStatusItems.Cold, null);
		this.deathcold.Enter("KillCold", delegate(TemperatureMonitor.Instance smi)
		{
			smi.KillCold();
		});
	}

	public GameStateMachine<TemperatureMonitor, TemperatureMonitor.Instance, IStateMachineTarget>.State satisfied;

	public GameStateMachine<TemperatureMonitor, TemperatureMonitor.Instance, IStateMachineTarget>.State hot;

	public GameStateMachine<TemperatureMonitor, TemperatureMonitor.Instance, IStateMachineTarget>.State deathhot;

	public GameStateMachine<TemperatureMonitor, TemperatureMonitor.Instance, IStateMachineTarget>.State cold;

	public GameStateMachine<TemperatureMonitor, TemperatureMonitor.Instance, IStateMachineTarget>.State deathcold;

	public new class Instance : GameStateMachine<TemperatureMonitor, TemperatureMonitor.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.temperature = Db.Get().Amounts.Temperature.Lookup(base.gameObject);
		}

		public bool IsHot()
		{
			return this.temperature.value > 318f;
		}

		public bool IsCold()
		{
			return this.temperature.value < 273f;
		}

		public bool IsFrozenSolid()
		{
			return this.temperature.value < 268f;
		}

		public bool IsOverHeated()
		{
			return this.temperature.value > 323f;
		}

		public void KillHot()
		{
			base.GetComponent<Health>().Kill(Db.Get().Deaths.Overheating);
		}

		public void KillCold()
		{
			base.GetComponent<Health>().Kill(Db.Get().Deaths.Frozen);
		}

		public AmountInstance temperature;
	}
}
