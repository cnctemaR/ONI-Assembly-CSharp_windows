using System;
using Klei.AI;

public class CalorieMonitor : GameStateMachine<CalorieMonitor, CalorieMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		base.serializable = true;
		this.satisfied.Transition(this.hungry, (CalorieMonitor.Instance smi) => smi.IsHungry());
		this.hungry.DefaultState(this.hungry.dirtyHands).Transition(this.satisfied, (CalorieMonitor.Instance smi) => smi.IsSatisfied()).ToggleExpression(Db.Get().Expressions.Hungry, null)
			.ToggleThought(Db.Get().Thoughts.Starving, null)
			.ToggleUrge(Db.Get().Urges.Eat);
		this.hungry.dirtyHands.ToggleUrge(Db.Get().Urges.WashHands).Transition(this.hungry.starving, (CalorieMonitor.Instance smi) => smi.IsStarving()).EventTransition(GameHashes.WorkCompleted, this.hungry.normal, (CalorieMonitor.Instance smi) => !smi.master.GetComponent<Effects>().HasEffect("DirtyHands"));
		this.hungry.normal.Transition(this.hungry.starving, (CalorieMonitor.Instance smi) => smi.IsStarving()).ToggleStatusItem(Db.Get().DuplicantStatusItems.Hungry, null);
		this.hungry.starving.Transition(this.hungry.normal, (CalorieMonitor.Instance smi) => !smi.IsStarving()).Transition(this.incapacitated, (CalorieMonitor.Instance smi) => smi.IsIncapacitated()).ToggleStatusItem(Db.Get().DuplicantStatusItems.Starving, null);
		this.incapacitated.Enter("Incapacitate", delegate(CalorieMonitor.Instance smi)
		{
			smi.Incapacitate();
		}).EventTransition(GameHashes.Healed, this.satisfied, null);
	}

	public GameStateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.State satisfied;

	public CalorieMonitor.HungryState hungry;

	public GameStateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.State incapacitated;

	public class HungryState : GameStateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.State dirtyHands;

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

		public bool IsIncapacitated()
		{
			return this.calories.value <= 0f;
		}

		public void Incapacitate()
		{
			base.GetComponent<Health>().Incapacitate(Db.Get().Deaths.Starvation);
		}

		public bool ShouldExitClinic()
		{
			return !this.IsStarving();
		}

		public AmountInstance calories;
	}
}
