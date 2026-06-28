using System;
using Klei.AI;

public class CalorieMonitor : GameStateMachine<CalorieMonitor, CalorieMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		base.serializable = true;
		this.satisfied.Transition(this.hungry, (CalorieMonitor.Instance smi) => smi.IsHungry(), UpdateRate.SIM_200ms);
		this.hungry.DefaultState(this.hungry.normal).Transition(this.satisfied, (CalorieMonitor.Instance smi) => smi.IsSatisfied(), UpdateRate.SIM_200ms).ToggleExpression(Db.Get().Expressions.Hungry, null)
			.ToggleThought(Db.Get().Thoughts.Starving, null)
			.ToggleUrge(Db.Get().Urges.Eat);
		this.hungry.normal.Transition(this.hungry.starving, (CalorieMonitor.Instance smi) => smi.IsStarving(), UpdateRate.SIM_200ms).ToggleStatusItem(Db.Get().DuplicantStatusItems.Hungry, null);
		this.hungry.starving.Transition(this.hungry.normal, (CalorieMonitor.Instance smi) => !smi.IsStarving(), UpdateRate.SIM_200ms).Transition(this.depleted, (CalorieMonitor.Instance smi) => smi.IsDepleted(), UpdateRate.SIM_200ms).ToggleStatusItem(Db.Get().DuplicantStatusItems.Starving, null);
		this.depleted.ToggleTag(GameTags.CaloriesDepleted).Enter(delegate(CalorieMonitor.Instance smi)
		{
			smi.Kill();
		});
	}

	public GameStateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.State satisfied;

	public CalorieMonitor.HungryState hungry;

	public GameStateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.State incapacitated;

	public GameStateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.State depleted;

	public class HungryState : GameStateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.State normal;

		public GameStateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.State starving;
	}

	public new class Instance : GameStateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.calories = Db.Get().Amounts.Calories.Lookup(base.gameObject);
		}

		private float GetCalories0to1()
		{
			return this.calories.value / this.calories.GetMax();
		}

		public bool IsHungry()
		{
			return this.GetCalories0to1() < 0.5f;
		}

		public bool IsStarving()
		{
			return this.GetCalories0to1() < 0.25f;
		}

		public bool IsSatisfied()
		{
			return this.GetCalories0to1() > 0.6f;
		}

		public bool IsDepleted()
		{
			return this.calories.value <= 0f;
		}

		public bool ShouldExitInfirmary()
		{
			return !this.IsStarving();
		}

		public void Kill()
		{
			DeathMonitor.Instance smi = base.gameObject.GetSMI<DeathMonitor.Instance>();
			if (smi != null)
			{
				base.gameObject.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Starvation);
			}
		}

		public AmountInstance calories;
	}
}
